
var EdmService = function () {
  return {
    createUploadLink: function (payload) {
      var options = {
        PartNumber: 1,
        IsMultiPart: true,
      }
      options = Object.assign({}, options, payload);
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'POST',
          url: '/Artifact/CreatePresignedLink',
          contentType: "application/json",
          data: JSON.stringify(options),
          success: function (response) {
            resolve(response)
          },
          error: function (response) {
            reject(response);
          }
        });
      });
    },
    getStreamFileUrl: function (documentGuid) {
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'GET',
          url: '/Artifact/GetPresignedLink?documentGuid=' + documentGuid,
          contentType: "application/json",
          success: function (response) {
            resolve(response)
          },
          error: function (response) {
            reject(response);
          },
        });
      });
    },
    upload: function (url, file, onProgress) {
      var timeStarted = Date.now();
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'PUT',
          xhr: function () {
            var xhr = new window.XMLHttpRequest();

            xhr.upload.addEventListener("progress", function (evt) {
              if (evt.lengthComputable && onProgress) {
                var percentage = (evt.loaded * 100) / evt.total;
                var progress = percentage.toFixed(2) + '%';
                var timeElapsed = Date.now() - timeStarted;
                var speed = evt.loaded / timeElapsed;
                var remaining = (evt.totaltotal - evt.loaded) / speed;
                onProgress({ progress: progress, speed: speed, remaining: remaining });
              }
            }, false);
            return xhr;
          },
          url: url,
          data: file,
          contentType: file.type,
          processData: false,
          success: function (response, textStatus, request) {
            resolve({
              data: response,
              etag: request.getResponseHeader('etag')
            })
          },
          error: function (response) {
            reject(response);
          }
        });
      });
    },
    aliveConfirm: function (documentGuid) {
      var options = { documentGuid: documentGuid};
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'POST',
          url: '/Artifact/AliveConfirm',
          contentType: "application/json",
          data: JSON.stringify(options),
          success: function (response) {
            resolve(response)
          },
          error: function (response) {
            reject(response);
          }
        });
      });
    },
    cancelUploadMultiPart: function (payload) {
      var options = {
        DocumentGuid: null,
        UploadId: null,
      }
      options = Object.assign({}, options, payload);
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'POST',
          url: '/Artifact/CancelUploadMultiPart',
          contentType: "application/json",
          data: JSON.stringify(options),
          success: function (response) {
            resolve(response)
          },
          error: function (response) {
            reject(response);
          }
        });
      });
    },
    updatePathEtags: function (payload) {
      var options = {
        DocumentGuid: null,
        PartETags: null,
      }
      options = Object.assign({}, options, payload);
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'POST',
          url: '/Artifact/UpdatePathEtags',
          contentType: "application/json",
          data: JSON.stringify(options),
          success: function (response) {
            resolve(response)
          },
          error: function (response) {
            reject(response);
          }
        });
      });
    },
    createInfo: function (fileName, fileSize) {
      var options = {
        DocumentName: fileName,
        Extension: '.' + fileName.split('.').pop().toLowerCase(),
        FileSize: fileSize || 0,
      }
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'POST',
          url: '/Artifact/CreateInfo',
          contentType: "application/json",
          data: JSON.stringify(options),
          success: function (response) {
            resolve(response)
          },
          error: function (response) {
            reject(response);
          }
        });
      });
    },
  };
};

var EDMHelper = function () {
  var uploadInfo = { progress: 0, speed: 0, remaining: 0 };
  var presignedLinkResult = null;
  var service = new EdmService();

  function upload(file, options) {
    options = Object.assign({}, {
      onProgress: $.noop
    }, options);
    
    return new Promise(function (resolve, reject) {
      service.createInfo(file.name, file.size)
        .then(function (res) {
          return res.DocumentGuid
        }).then(function (documentGuid) {
          service.createUploadLink({
            FileName: file.name,
            IsMultiPart: false,
            DocumentGuid: documentGuid
          }).then(function (res) {
            presignedLinkResult = res;
            service.upload(res.Url, file, function (result) {
              uploadInfo = result.data;
              options.onProgress(uploadInfo);
            }).then(function () {
              resolve(res.DocumentGuid)
            }).catch(reject);
          }).catch(reject);
        }).catch(reject);
    });
  };

  function getStreamUrl() {
    return service.getStreamFileUrl(presignedLinkResult.documentGuid).then(function (res) { return res });
  };

  return {
    upload: upload,
    getStreamUrl: getStreamUrl,
    uploadInfo: function () {
      return uploadInfo;
    }
  }
};

var EDMChunkUploadHelper = function (fileName) {
  var partNumber = 0;
  var eTags = [];
  var currentBlobsLength = 0;
  var recorder = null;
  var promises = [];
  var isFinishRecorded = false;

  var options = {
    FileName: fileName,
    IsMultiPart: true,
    DocumentGuid: null,
    UploadId: null,
    PrevETags: null,
  };
  var service = new EdmService();
  function setOptions(params) {
    options = Object.assign({}, options, params);
  };

  var createUploadLink = function(ops) {
    var promises = [];
    if (ops.partNumber == 1) {
      promises.push(service.createInfo(options.FileName)
        .then(function (res) {
          setOptions({ DocumentGuid: res.DocumentGuid });
        }));
    }

    return new Promise(function (resolve) {
      Promise.all(promises).then(function () {
        var payload = Object.assign({}, options, {
          PartNumber: ops.partNumber,
          IsFinished: ops.isFinished ?? false,
        });

        service.createUploadLink(payload).then(function (res) {
          if (!ops.isFinished) {
            var data = Object.assign({}, res);
            delete data.Url;
            setOptions(data);
            service.upload(res.Url, ops.blobs).then(function (uploadRes) {
              eTags.push(uploadRes.etag);
              setOptions({
                PrevETags: eTags.map(function (tag, index) {
                  return (index + 1) + ',' + tag;
                }).join(','),
              });
              service.updatePathEtags({
                DocumentGuid: options.DocumentGuid,
                PartETags: options.PrevETags
              }).then(function () {
                resolve(uploadRes);
              });
            });
          } else {
            resolve();
          }
        });
      });
    });
  };

  function startRecordUpload(recorderInstance, saveCallback, errorCallback) {
    if (!recorder && !recorderInstance) {
      return;
    }
    recorder = recorder ? recorder : recorderInstance;
    checkAlive();
    (function looper() {
      if (!recorder || isFinishRecorded) {
        return;
      }
      if (!window.navigator.onLine) {
        errorCallback && errorCallback({ type: 'OFFLINE' });
        return;
      }
      var internal = recorder.getInternalRecorder();
      if (internal && internal.getArrayOfBlobs) {
        var arrayOfBlobs = internal.getArrayOfBlobs();
        if (arrayOfBlobs.length > currentBlobsLength) {
          var splitBlob = arrayOfBlobs.slice(currentBlobsLength);
          var blobs = new Blob(splitBlob, {
            type: 'video/webm',
          });
          var blobSizeMB = blobs.size / (1024 * 1024);
          if (blobSizeMB >= 5) {
            currentBlobsLength = arrayOfBlobs.length;
            partNumber += 1;
            var payload = { blobs: blobs, partNumber: partNumber };
            Promise.all(promises).then(function () {
              promises = [createUploadLink(payload)];
              saveCallback(promises[0], payload.partNumber);
            });
          }
        }
      }
      setTimeout(looper, 1000);
    })();
  };

  function finishRecordUpload(recorderInstance, callback) {
    isFinishRecorded = true;
    return new Promise(function (resolve, reject) {
      if (!recorder && !recorderInstance) {
        reject();
      }
      if (partNumber === 0 || !navigator.onLine) {
        recorder = null;
        resolve();
        return;
      }
      try {
        recorder = recorder ? recorder : recorderInstance;
        var internal = recorder.getInternalRecorder();
        if (internal && internal.getArrayOfBlobs) {
          var arrayOfBlobs = internal.getArrayOfBlobs();
          var splitBlob = arrayOfBlobs.slice(currentBlobsLength);
          if (splitBlob.length) {
            var blobs = new Blob(splitBlob, {
              type: 'video/webm',
            });
            callback && callback();
            Promise.all(promises).then(function () {
              return new Promise(function (resolve) {
                setTimeout(function () {
                  resolve();
                }, 500);
              })
            }).then(function () {
              Promise.all(promises).then(function () {
                partNumber += 1;
                var payload = { blobs: blobs, partNumber: partNumber };
                promises = [createUploadLink(payload)];
                Promise.all(promises).then(function () {
                  payload.isFinished = true;
                  createUploadLink(payload).then(function (res) {
                    recorder = null;
                    resolve();
                  });
                });
              });
            });
          }
        }
      } catch (err) {
        recorder = null;
        reject(err)
      }
    });
  };

  function checkAlive() {
    (function looper() {
      if (!recorder || !recorder.getInternalRecorder()) {
        return;
      }
      if (options.DocumentGuid) {
        service.aliveConfirm(options.DocumentGuid);
      }
      setTimeout(looper, 5000);
    })();
  }

  function getStreamUrl() {
    return service.getStreamFileUrl(options.documentGuid).then(function (res) { return res });
  };

  return {
    startRecordUpload: startRecordUpload,
    finishRecordUpload: finishRecordUpload,
    getStreamUrl: getStreamUrl,
    getOptions: function () {
      return options;
    },
    getPartNumber: function () {
      return partNumber;
    }
  };
};
