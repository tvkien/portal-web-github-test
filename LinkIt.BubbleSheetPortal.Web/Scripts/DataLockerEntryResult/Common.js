function buildIconInfo(item, response, isCustomSubScores, fullResponse) {
  var infoIcon = '';
  if (item.MetaData.Description) {
    infoIcon += item.MetaData.Description;
  }

  if (item.MetaData.DataType == "Numeric") {

    if (item.MetaData.MaxValue != null && item.MetaData.MinValue != null) {
      var format = item.MetaData.DecimalScale;
      infoIcon += '<p>Min Value: ' + item.MetaData.MinValue.toFixed(format) + '</p>';
      infoIcon += '<p>Max Value: ' + item.MetaData.MaxValue.toFixed(format) + '</p>';
    }
    if (item.MetaData.DecimalScale != null) {
      infoIcon += '<p># Of Decimal Places: ' + item.MetaData.DecimalScale + '</p>';
    }
    infoIcon += renderColorTooltip(item, response, isCustomSubScores, fullResponse);
  } else if (item.MetaData.DataType == "Numeric" && item.MetaData.FormatOption == "LabelValueText") {
     infoIcon += renderColorTooltip(item, response, isCustomSubScores, fullResponse);
  }
  if ((item.MetaData.DataType == "FreeForm"
    || item.MetaData.DataType == "Alphanumeric"
    || item.MetaData.DataType == "Alphabetic")
    && item.MetaData.MaxLength) {
    infoIcon += '<p>Max Characters: ' + item.MetaData.MaxLength + '</p>';
  }
  return infoIcon;
}

function renderColorTooltip(item, response, isCustomSubScores, fullResponse) {
  var textResponse = '';
  if (isCustomSubScores) {
    if (fullResponse) {
      var subscoreName = item.Id.split('::')[0];
      var settings = fullResponse.PerformanceBandSettingSubScores;
      settings && settings.forEach(function(setting){
        if (compareScoreName(item.ScoreName, setting.ScoreType) && subscoreName === setting.SubScoreName) textResponse += renderColorItem(setting);
      });
    }
  } else {
    var settings = response.PerformanceBandSettingScores;
    settings && settings.forEach(function(setting){
      if(compareScoreName(item.ScoreName, setting.ScoreType)) textResponse += renderColorItem(setting);
    });
  }
  return textResponse;
}

function compareScoreName(scoreName, scoreTypePbs) {
   if (scoreName === 'Percentile') scoreName = 'Percentage'
   return ('Score' + scoreName) === scoreTypePbs
}

// Function convert text bands to colors
function getPerformanceStyle(score) {
  if (score.color) {
    return {
      bgColor: score.color,
      color: getContrastColor(score.color)
    };
  }

  const length = score.colorBand;

  if (score.scoreIndex >= length) {
    return {
      bgColor: rgb(215,215,215),
      color: null
    };
  }

  if (score.scoreIndex === 0) {
    return {
      bgColor: convertColorPerformance(1, 1),
      color: getContrastColor(convertColorPerformance(1, 1))
    };
  }

  if (score.scoreIndex === length - 1) {
    return {
      bgColor: convertColorPerformance(0, 1),
      color: getContrastColor(convertColorPerformance(0, 1))
    };
  }

  return {
    bgColor: convertColorPerformance(length - score.scoreIndex - 1, length - 1),
    color: getContrastColor(convertColorPerformance(length - score.scoreIndex - 1, length - 1))
  };
}

function getContrastColor(color) {
  let rgbValue = [];
  switch (true) {
    case color.indexOf('hsl') === 0:
      rgbValue = hslToRgbValue(color);
      break;

    case /^#[0-9A-F]{6}$/i.test(color):
      rgbValue = hexToRgbValue(color);
      break;

    default:
      rgbValue = rgbToRgbValue(color)
      break;
  }

  const l = luminanace(rgbValue[0], rgbValue[1], rgbValue[2]);
  const lText = 0; // rgb(0, 0, 0)
  const ratio = (lText + 0.05) / (l + 0.05);
  if (ratio < 1 / 7) {
    return 'rgb(0, 0, 0)';
  }
  return 'rgb(255, 255, 255)';
}

function convertColorPerformance(CutOff, MaxCutOff) {
  let percent = 0;
  if (MaxCutOff !== 0) {
    percent = (CutOff * 100) / MaxCutOff;
  }
  return percentToColorPerformance(percent || 0, 0, 100);
}

function percentToColorPerformance(percent, startColor, endColor) {
  const a = percent / 100;
  const b = (endColor - startColor) * a;
  const c = b + startColor;
  return 'hsl(' + c + ', 100%, 50%)';
}

function hslToRgbValue(color) {
  const hsl = color.split(/\(([^)]+)\)/)[1].replace(/ /g, '').replace(/%/gi, '');
  const h = parseFloat(hsl.split(',')[0], 10) / 360;
  const s = parseFloat(hsl.split(',')[1], 10) / 100;
  const l = parseFloat(hsl.split(',')[2], 10) / 100;
  let r, g, b;

  if (s === 0) {
    r = g = b = l; // achromatic
  } else {
    const hue2rgb = function hue2rgb(p, q, t) {
      if (t < 0) t += 1;
      if (t > 1) t -= 1;
      if (t < 1 / 6) return p + (q - p) * 6 * t;
      if (t < 1 / 2) return q;
      if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
      return p;
  };

    const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
    const p = 2 * l - q;
    r = hue2rgb(p, q, h + 1 / 3);
    g = hue2rgb(p, q, h);
    b = hue2rgb(p, q, h - 1 / 3);
  }

  return [Math.round(r * 255), Math.round(g * 255), Math.round(b * 255)];

}

function rgbToRgbValue(color) {
  const sep = color.indexOf(',') > -1 ? ',' : ' ';
  const rgb = color.substr(4).split(')')[0].split(sep);
  return [parseFloat(rgb[0]), parseFloat(rgb[1]), parseFloat(rgb[2])]
}

function hexToRgbValue(color) {
  const shorthandRegex = /^#?([a-f\d])([a-f\d])([a-f\d])$/i;
  const hex = color.replace(shorthandRegex, function(m, r, g, b) {
    return r + r + g + g + b + b;
  });
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
  return [parseInt(result[1], 16), parseInt(result[2], 16), parseInt(result[3], 16)];
}

function luminanace(r, g, b) {
  var a = [r, g, b].map(function(v) {
    v /= 255;
    return v <= 0.03928 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4);
  });
  return a[0] * 0.2126 + a[1] * 0.7152 + a[2] * 0.0722;
}

// End

function renderColorItem(setting) {
  var result = '';
  var colors = setting.Color ? setting.Color.split(";") : [];
  var bands = setting.Bands ? setting.Bands.split(";") : [];
  var cutoffs = setting.Cutoffs ? setting.Cutoffs.split(",") : [];
  if (colors.length > 0) {
      colors.forEach(function(color, index) {
      var numberLabel = '';
      switch(index) {
        case 0:
          numberLabel = `[${toFixed(cutoffs[0])}+]`;
          break;
        case colors.length - 1:
          numberLabel = `[Below ${toFixed(cutoffs[cutoffs.length - 1])}]`;
          break;
        default:
          numberLabel = `[${toFixed(cutoffs[index])} - ${getRoundNumberPBSDisplay(cutoffs[index - 1])}]`;
      }
      result += `<div class='description-psbcolors' style='display: flex; margin-bottom: 4px;'>
        <div class='psbcolors' style='flex-shrink:0; margin-right: 8px; background-color: ${color}; width: 16px; height: 16px;'></div> ${bands[index]} ${numberLabel}
      </div>`;
    });
  } else {
    bands.forEach(function(band, index) {
      var bandColorStyle = getPerformanceStyle({
        scoreIndex: index,
        colorBand: bands.length
      })
      var numberLabel = '';
      switch(index) {
        case 0:
          numberLabel = `[${toFixed(cutoffs[0])}+]`;
          break;
        case bands.length - 1:
          numberLabel = `[Below ${toFixed(cutoffs[cutoffs.length - 1])}]`;
          break;
        default:
          numberLabel = `[${toFixed(cutoffs[index])} - ${getRoundNumberPBSDisplay(cutoffs[index - 1])}]`;
      }
      result += `<div class='description-psbcolors' style='display: flex; margin-bottom: 4px;'>
        <div class='psbcolors' style='flex-shrink:0; margin-right: 8px; width: 16px; height: 16px; background-color: ${bandColorStyle.bgColor}; color: ${bandColorStyle.color}'></div> ${bands[index]} ${numberLabel}
      </div>`;
    });
  }
  
  return result;
}

function toFixed(x) {
  if (!x) return '0';
  if (Math.abs(x) < 1.0) {
    const e = parseInt(x.toString().split('e-')[1]);

    if (e) {
      x *= Math.pow(10, e - 1);
      x = '0.' + new Array(e).join('0') + x.toString().substring(2);
    }
  } else {
    let e = parseInt(x.toString().split('+')[1]);
    if (e > 20) {
      e -= 20;
      x /= Math.pow(10, e);
      x += new Array(e + 1).join('0');
    }
  }
  return x;
}

function getRoundNumberPBSDisplay(number) {
  return toFixed(getRoundNumberPBS(number));
}

function getRoundNumberPBS(number) {
  if (!number && number !== 0) {
    return number;
  }
  const stringNumber = parseFloat(number).toString();
  const parts = stringNumber.split('.');
  const x = (parts[1] && parts[1].length) || 1;
  const roundNumber = Math.pow(10, -x);
  const result = parseFloat(stringNumber) - roundNumber;
  return parseFloat(result.toFixed(x));
}
