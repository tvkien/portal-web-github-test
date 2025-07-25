(function (context, $) {
  // Dependencies: jQuery
  if (!$.fn) {
    throw new Error("LNKT-Table: Required dependencies (jQuery) are missing.");
  }
  if ($.fn.simpleTable) {
    console.warn("LNKT-Table: Duplicate import detected.");
  }

  const colors = [
    "#D64541",
    "#8E44AD",
    "#27AE60",
    "#F39C12",
    "#2980B9",
    "#16A085",
    "#E74C3C",
    "#2C3E50",
    "#F1C40F",
    "#9B59B6",
    "#34495E",
    "#E67E22",
    "#1ABC9C",
    "#3498DB",
    "#9C27B0",
    "#C0392B",
    "#FF5722",
    "#673AB7",
    "#FF6F61",
    "#4A90E2"
  ];


  function debounce(func, delay) {
    var timeoutId;
    return function () {
      var context = this;
      var args = arguments;

      clearTimeout(timeoutId);
      timeoutId = setTimeout(function () {
        func.apply(context, args);
      }, delay);
    };
  }

  // Function to create DOM from JSON tree with event handling
  $.jsonToDOM = function (node) {
    var element = document.createElement(node.tag || "div");
    if (node.attributes) {
      Object.entries(node.attributes).forEach(([key, value]) => {
        element.setAttribute(key, value);
      });
    }
    if (node.class) {
      element.className = node.class;
    }
    if (node.content) {
      element.innerHTML = node.content;
    } else if (node.html) {
      element.innerHTML = node.html;
    }
    if (node.event) {
      element.addEventListener(node.event.type, node.event.handler);
    }
    if (node.children && !node.html) {
      node.children.forEach((childNode) => {
        var childElement = $.jsonToDOM(childNode);
        element.appendChild(childElement);
      });
    }

    return element;
  };

  var filterChanged = false;

  function onFilterClick(col) {
    var that = this;
    var th = this.$.root.find('th[field="' + col.field + '"]');
    th.find('.filterRow').show();
    th.find('.filterSearch input').val('');
    var offset = th.offset();
    var dropdown = th.find('.filterDropdown');
    dropdown.css('width', th.width() + 2)
    dropdown.appendTo(document.body);
    var left = offset.left + th.width() - dropdown[0].getBoundingClientRect().width + 2;
    if (left < 0) {
      left = 0;
    }
    dropdown.css({
      top: offset.top + th.height(),
      left
    })
    var checkToCloseFilter = function(e) {
      if ($(e.target).closest('.filterDropdown').length === 0) {
        $(document).off('click', checkToCloseFilter);
        dropdown.appendTo(th);
        if (filterChanged) {
          filterChanged = false;
          that.options.pagination.pageIndex = 0;
          that.fetchData();
        }
      }
    }
    $(document).on('click', checkToCloseFilter);
  }

  function updateFilter(th, col) {
    if (!this.options.filters[col.filterField]) return;
    if (this.options.filters[col.filterField].length === this.options.dropDownFilters[col.filterField].length) {
      th.find('.filterRow input').prop('checked', true);
    } else {
      th.find('.filterRow input').prop('checked', false);
      this.options.filters[col.filterField].forEach(function(e) {
        th.find('.filterRow[id="' + e + '"] input').prop('checked', true);
      })
    }
    filterChanged = true;
  }

  function initFilter() {
    var that = this;
    that.options.filterEndpoint && $.ajax({
      url: that.options.filterEndpoint,
      data: that.options.requestData || {},
      success: function (response) {
        that.options.dropDownFilters = that.options.filterMapper.apply(that, [response]);
        that.$.root.find('th .filterDropdown').remove();
        that.options.columns.forEach(function(col) {
          if (col.allowFilter === false || !Array.isArray(that.options.dropDownFilters[col.filterField])) return;
          var th = that.$.root.find('th[field="' + col.field + '"]');
          th.append($.jsonToDOM({
            class: 'filterDropdown',
            children: [
              {
                class: 'filterSearch form',
                children: [
                  {
                    tag: 'input',
                    attributes: {
                      type: 'text'
                    },
                    event: {
                      type: 'keyup',
                      handler: function(e) {
                        $(document.documentElement).find('body > .filterDropdown .filterRow').not('.selectAll').each(function(_, el) {
                          if (el.textContent.toLowerCase().includes(e.target.value.toLowerCase())) {
                            $(el).show();
                          } else {
                            $(el).hide();
                          }
                        })
                      }
                    }
                  }
                ]
              },
              {
                class: 'filterItem',
                children: [
                  {
                    class: 'filterRow selectAll',
                    children: [
                      {
                        tag: 'input',
                        attributes: {
                          type: 'checkbox'
                        }
                      },
                      {
                        tag: 'span',
                        content: 'Select All'
                      }
                    ],
                    event: {
                      type: 'click',
                      handler: function(e) {
                        var value = $(e.target).closest('.filterRow').find('input').is(':checked');
                        if (e.target.tagName !== 'INPUT') {
                          value = !value;
                        }
                        if (!Array.isArray(that.options.filters[col.filterField])) {
                          that.options.filters[col.filterField] = [];
                        }
                        if (value) {
                          that.options.filters[col.filterField] = that.options.dropDownFilters[col.filterField].map(function (e) { return e.Id; });
                        } else {
                          that.options.filters[col.filterField] = [];
                        }
                        updateFilter.apply(that, [$(document).find('body > .filterDropdown'), col])
                      }
                    }
                  }
                ].concat(that.options.dropDownFilters[col.filterField].map(function (opt) {
                  return {
                    class: 'filterRow',
                    attributes: {
                      id: opt.Id
                    },
                    children: [
                      {
                        tag: 'input',
                        attributes: {
                          type: 'checkbox'
                        }
                      },
                      {
                        tag: 'span',
                        content: opt.Name
                      }
                    ],
                    event: {
                      type: 'click',
                      handler: function(e) {
                        var value = $(e.target).closest('.filterRow').find('input').is(':checked');
                        if (e.target.tagName !== 'INPUT') {
                          value = !value;
                        }
                        $(e.target).closest('.filterRow').find('input').prop('checked', value);
                        if (!Array.isArray(that.options.filters[col.filterField])) {
                          that.options.filters[col.filterField] = [];
                        }
                        if (value) {
                          that.options.filters[col.filterField].push(opt.Id);
                        } else {
                          that.options.filters[col.filterField] = that.options.filters[col.filterField].filter(function(e) { return e !== opt.Id; });
                        }
                        updateFilter.apply(that, [th, col])
                      }
                    }
                  }
                }))
              }
            ]
          }))
        })
      },
      error: function (xhr, status, error) {
        console.error("Error:", error);
      },
    });
  }

  function createTable(container, options) {
    function getVisibleColumns() {
      return options.columns.filter(function (col) {
        return col.visible !== false;
      });
    }

    var element = $(container);
    var visibleColumns = getVisibleColumns();

    visibleColumns.forEach(function(col) {
      if (col.sort) {
        options.pagination.sortBy = col.field;
        options.pagination.sortDirection = col.sort;
      }
    })

    element
      .addClass("lnkt-instance")
      .html("")
      .append(
        $.jsonToDOM({
          class: "lnkt-table-container",
          children: [
            {
              class: "lnkt-table-control",
              children: [
                {
                  class: "lnkt-table-control-left",
                  children: Array.isArray(options.leftControl) ? options.leftControl.map(function (e) {
                    if (e.type === 'switch') {
                      return {
                        class: 'switch-wrapper',
                        children: [
                          {
                            class: 'form-switch-label',
                            content: e.label || 'Off'
                          },
                          {
                            class: 'form-switch',
                            children: [
                              {
                                tag: 'input',
                                class: 'form-check-input',
                                attributes: {
                                  type: 'checkbox'
                                },
                                event: {
                                  type: 'change',
                                  handler: function (event) {
                                    if (!event.target.disabled && e.onChange) {
                                      e.onChange({
                                        value: !$(event.target).hasClass('input-checked-v2'),
                                        target: event.target,
                                        labelTarget: $(event.target).closest('.lnkt-table-control-left').find('.form-switch-label')[0]
                                      })
                                    }
                                  }
                                }
                              }
                            ]
                          }
                        ]
                      }
                    } else {
                      return e;
                    }
                  }) : []
                },
                {
                  class: "lnkt-table-control-right",
                  children: Array.isArray(options.rightControl) ? options.rightControl.map(function (e) {
                    if (e.type === 'search') {
                      return {
                        class: 'data-search',
                        children: [
                          {
                            tag: 'input',
                            attributes: {
                              type: 'text'
                            },
                            event: {
                              type: 'keyup',
                              handler: debounce(function (e) {
                                options.generalSearch = $(e.target).val().trim();
                                options.pagination.pageIndex = 0;
                                options.rightControl.onSearch && options.rightControl.onSearch(e);
                                fetchData();
                              }, $.fn.lnktTable.debounceTime)
                            }
                          }
                        ]
                      }
                    } else {
                      return e
                    }
                  }) : []
                },
              ],
            },
            {
              class: 'lnkt-table-for-loading',
              children: [
                {
                  class: "lnkt-table-main",
                  attributes: {
                    style: 'max-height: ' + options.maxHeight
                  },
                  children: [
                    {
                      tag: "table",
                      class: "lnkt-table",
                      attributes: {
                        border: "1",
                      },
                      children: [
                        {
                          tag: 'thead',
                          children: [
                            {
                              tag: "tr",
                              class: "lnkt-table-header",
                              children: visibleColumns.map(function (col) {
                                var children = [
                                  {
                                    content: col.title || col.field,
                                  },
                                ];
                                children.push({
                                  class: "lnkt-table-icon-headers",
                                  children: [
                                    {
                                      class: "lnkt-table-icon-header sort " + (col.sort ? '' : 'hide'),
                                      html: '<svg viewBox="0 0 512 512"><path d="M233.4 105.4c12.5-12.5 32.8-12.5 45.3 0l192 192c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0L256 173.3 86.6 342.6c-12.5 12.5-32.8 12.5-45.3 0s-12.5-32.8 0-45.3l192-192z"/></svg>',
                                    },
                                    {
                                      class: "lnkt-table-icon-header filter " + (col.allowFilter === false ? 'hide' : ''),
                                      html: '<svg viewBox="0 0 512 512"><path fill="currentColor" d="M488 0H24C2.7 0-8 25.9 7.1 41L192 225.9V432c0 7.8 3.8 15.2 10.2 19.7l80 56C298 518.7 320 507.5 320 488V225.9l184.9-185C520 25.9 509.3 0 488 0z"/></svg>',
                                      event: {
                                        type: 'click',
                                        handler: function (e) {
                                          $(document.body).click();
                                          onFilterClick.apply(element.data(), [col]);
                                          e.preventDefault();
                                          e.stopPropagation();
                                        }
                                      }
                                    },
                                  ]
                                });
                                return {
                                  tag: "th",
                                  attributes: {
                                    field: col.field
                                  },
                                  children: [
                                    {
                                      children,
                                      event: {
                                        type: 'click',
                                        handler: function (e) {
                                          if (options.pagination.sortBy === col.field) {
                                            options.pagination.sortDirection = options.pagination.sortDirection === 'asc' ? 'desc' : 'asc';
                                          } else {
                                            options.pagination.sortBy = col.field;
                                            options.pagination.sortDirection = 'asc';
                                          }
                                          options.pagination.pageIndex = 0;
                                          renderSortHeader.apply(element.data());
                                          fetchData.apply(element.data());
                                        }
                                      }
                                    }
                                  ]
                                };
                              }),
                            }
                          ],
                        },
                        {
                          tag: 'tbody',
                          children: Array.isArray(options.dataItems) ? options.dataItems.map(function (item) {
                            return {
                              tag: 'tr',
                              children: visibleColumns.map(function (col) {
                                return {
                                  tag: 'td',
                                  content: item[col.field]
                                }
                              })
                            }
                          }) : []
                        },
                        {
                          tag: 'tfoot',
                          children: [
                            {
                              tag: "tr",
                              class: "lnkt-table-no-result " + (options.dataItems && options.dataItems.length ? 'hide' : ''),
                              children: [
                                {
                                  tag: "td",
                                  attributes: {
                                    colspan: visibleColumns.length,
                                  },
                                  content: options.noResultText,
                                },
                              ],
                            }
                          ]
                        }
                      ]
                    },
                  ],
                },
                {
                  class: "lnkt-table-footer",
                  children: [
                    {
                      class: "lnkt-table-legend",
                    },
                    {
                      class: "lnkt-table-total-records",
                      content: "Showing 0 to 0 of 0 entries",
                    },
                    {
                      class: "lnkt-table-paging",
                      children: [
                        {
                          class: "lnkt-table-paging-entry",
                          children: [
                            {
                              content: 'Shown entries'
                            },
                            {
                              tag: 'select',
                              children: Array.isArray(options.pageSizes) ? options.pageSizes.map(function (e, index) {
                                var opt = {
                                  tag: 'option',
                                  attributes: {
                                    value: e
                                  },
                                  content: e
                                }
                                if (index === 0) {
                                  opt.attributes.selected = true
                                }
                                return opt;
                              }) : [],
                              event: {
                                type: 'change',
                                handler: function (e) {
                                  options.pagination.pageSize = e.target.value;
                                  options.pagination.pageIndex = 0;
                                  fetchData();
                                }
                              }
                            }
                          ]
                        },
                        {
                          class: "lnkt-table-paging-action",
                          children: [
                            {
                              class: "lnkt-table-previous disabled",
                              event: {
                                type: 'click',
                                handler: function(e) {
                                  if (!e.target.classList.contains('disabled')) {
                                    options.pagination.pageIndex -= 1;
                                    fetchData();
                                  }
                                }
                              }
                            },
                            {
                              class: "lnkt-table-next disabled",
                              event: {
                                type: 'click',
                                handler: function(e) {
                                  if (!e.target.classList.contains('disabled')) {
                                    options.pagination.pageIndex += 1;
                                    fetchData();
                                  }
                                }
                              }
                            }
                          ]
                        },
                        {
                          class: "lnkt-table-filter-info"
                        }
                      ]
                    }
                  ]
                },
                {
                  class: "lnkt-table-loading",
                  attributes: {
                    style: "display: none;",
                  },
                  html: '<div class="block-loading"><div class="loading-ring-container"><div class="outer-ring"><div></div><div></div><div></div><div></div></div><div class="middle-ring"><div></div><div></div><div></div><div></div></div><div class="inner-ring"><div></div><div></div><div></div><div></div></div></div><span style="font-size: 20px;">Loading ...</span></div>',
                }
              ]
            }
          ],
        })
      );

    function setIsLoading(val) {
      options.isLoading = val;
      options.onLoadingChange && options.onLoadingChange.apply(element.data(), [val]);
      val
        ? element.find(".lnkt-table-loading").show()
        : element.find(".lnkt-table-loading").hide();
    }

    function renderData() {
      if (options.dataItems && options.dataItems.length) {
        element.find(".lnkt-table tbody tr").remove();
        var rows = options.dataItems.map(function (item) {
          var isSelected = options.allowSelecting && options.rowKey && options.selected && item[options.rowKey] === options.selected[options.rowKey];
          return $.jsonToDOM({
            tag: 'tr',
            class: isSelected ? 'selected' : '',
            attributes: {
              key: options.rowKey ? item[options.rowKey] : '',
            },
            children: getVisibleColumns().map(function (col) {
              return {
                tag: 'td',
                content: item[col.field]
              }
            }),
            event: {
              type: 'click',
              handler: function() {
                if (options.allowSelecting) {
                  if (options.selected && options.selected[options.rowKey] === item[options.rowKey]) {
                    options.selected = null;
                    element.find(".lnkt-table tbody tr").removeClass('selected');
                  } else {
                    options.selected = item;
                    element.find(".lnkt-table tbody tr").removeClass('selected');
                    element.find('.lnkt-table tbody tr[key="' + item[options.rowKey] + '"]').addClass('selected');
                  }
                  renderFooterFilter.apply(element.data());
                  options.onRowSelected && options.onRowSelected.apply(element.data());
                }
              }
            }
          })
        });
        element.find(".lnkt-table tbody").append($(rows));
        element.find(".lnkt-table-no-result").hide();
      } else {
        element.find(".lnkt-table tbody tr").remove();
        element.find(".lnkt-table-no-result").show();
      }
      options.onTableRenderComplete && options.onTableRenderComplete.apply(element.data());
    }

    function renderTotalRecords() {
      var startIndex = options.pagination.pageSize * options.pagination.pageIndex + 1;
      var endIndex = startIndex + options.dataItems.length - 1;
      if (options.dataItems.length === 0) {
        startIndex = 0;
        endIndex = 0;
      }
      this.$.totalRecord.text("Showing " + startIndex + " to " + endIndex + " of " + (options.totalRecords || 0) + " entries");
      this.$.nextButton.removeClass('disabled');
      this.$.previousButton.removeClass('disabled');
      if (endIndex == options.totalRecords) {
        this.$.nextButton.addClass('disabled');
      }
      if (startIndex < 2) {
        this.$.previousButton.addClass('disabled');
      }
    }

    function renderFooterFilter() {
      var that = this;
      var displayFilter = [];
      visibleColumns.forEach(function(col, columnIndex) {
        if (that.options.filters[col.filterField] && that.options.filters[col.filterField].length) {
          displayFilter.push({
            isTitle: true,
            columnIndex,
            text: (col.title || col.field) + ': '
          });
          that.options.filters[col.filterField].forEach(function(filterVal) {
            var item = that.options.dropDownFilters[col.filterField].find(function(e) {
              return filterVal === e.Id
            })
            displayFilter.push({
              columnIndex,
              text: item.Name,
              value: item.Id,
              filterField: col.filterField
            });
          })
        }
      });
      if (displayFilter.length) {
        displayFilter.unshift({ isTitle: true, text: 'Filter: ' });
        displayFilter.push({
          isTitle: true,
          isClearAll: true
        });
      }
      if (that.options.selected) {
        displayFilter.push({
          isTitle: true,
          text: 'Selected: '
        });
        displayFilter.push({
          columnIndex: visibleColumns.length,
          text: that.options.selected[that.options.rowDisplayKey],
          isSelected: true
        });
      }
      that.$.table.find('.lnkt-table-icon-header.filter').removeClass('is-applied');
      displayFilter.forEach(function (e) {
        if (!e.isTitle && e.columnIndex < visibleColumns.length) {
          var field = visibleColumns[e.columnIndex].field;
          that.$.table.find('th[field="' + field + '"] .filter').addClass('is-applied');
        }
      })
      that.$.root.find('.lnkt-table-filter-item,.lnkt-table-filter-group').remove();
      if (displayFilter.length === 1) return;
      that.$.filterInfo.after($(displayFilter.map(function (filter) {
        return $.jsonToDOM(
          filter.isTitle ? {
            content: filter.text || 'Clear All',
            class: 'lnkt-table-filter-group ' + (filter.isClearAll ? 'clearAll' : ''),
            attributes: filter.isClearAll ? {} : {
              style: 'color:' + colors[filter.columnIndex]
            },
            event: {
              type: 'click',
              handler: function() {
                if (!filter.isClearAll) return;
                var instance = element.data();
                instance.options.filters = {};
                instance.renderFooterFilter();
                instance.fetchData();
                setTimeout(function() {
                  instance.$.root.find('th .filterRow input').prop('checked', false);
                }, 200);
              }
            }
          } : {
            class: 'lnkt-table-filter-item',
            attributes: {
              style: 'color:white;background:' + colors[filter.columnIndex]
            },
            children: [
              {
                tag: 'span',
                content: filter.text,
              },
              {
                tag: 'span',
                content: 'x',
                event: {
                  type: 'click',
                  handler: function(e) {
                    var instance = element.data();
                    if (filter.isSelected) {
                      options.selected = null;
                      element.find(".lnkt-table tbody tr").removeClass('selected');
                      instance.renderFooterFilter();
                      options.onRowSelected && options.onRowSelected.apply(element.data());
                    } else {
                      options.filters[filter.filterField] = options.filters[filter.filterField].filter(function(id) {
                        return id !== filter.value;
                      });
                      instance.renderFooterFilter();
                      that.fetchData();
                      setTimeout(function() {
                        instance.options.columns.forEach(function(col) {
                          var th = instance.$.root.find('th[field="' + col.field + '"]');
                          if (col.allowFilter !== false && th.length) {
                            instance.updateFilter(th, col)
                          }
                        })
                      }, 200);
                    }
                  }
                }
              }
            ]
          }
        )
      })))
    }

    function renderSortHeader() {
      this.$.root.find('.lnkt-table-header th .sort').addClass('hide');
      var header = this.$.root.find('.lnkt-table-header th[field="' + this.options.pagination.sortBy + '"]');
      header.find('.sort').removeClass('hide asc desc').addClass(this.options.pagination.sortDirection);
    }

    function clearAll() {
      this.options.filters = {};
      this.options.dropDownFilters = {};
      this.options.selected = null;
      this.options.dataItems = [];
      this.$.previousButton.addClass('disabled');
      this.$.nextButton.addClass('disabled');
      renderData.apply(this);
      renderTotalRecords.apply(this);
      renderFooterFilter.apply(this);
      renderSortHeader.apply(this);
      options.onRowSelected && options.onRowSelected.apply(this);
    }

    function fetchData() {
      if (this.disabled === true) return;
      setIsLoading(true);
      var data = {
        filters: options.filters,
        generalSearch: options.generalSearch,
        pagination: options.pagination
      }
      $.ajax({
        url: options.endpoint,
        method: options.method || "GET",
        contentType: 'application/json',
        data: JSON.stringify(Object.assign(data, options.requestData || {})),
        success: function (response) {
          var that = element.data();
          options.dataItems = response.Data;
          options.onFetchDataSuccess && options.onFetchDataSuccess.apply(element.data());
          options.totalRecords = response.TotalRecords;
          renderData.apply(that);
          renderTotalRecords.apply(that);
          renderFooterFilter.apply(that);
          renderSortHeader.apply(that);
        },
        error: function (xhr, status, error) {
          console.error("Error:", error);
        },
      }).always(function () {
        setIsLoading(false);
      });
    }

    return {
      isLnktTable: true,
      $: {
        root: element,
        controlRight: element.find('.lnkt-table-control-right'),
        controlLeft: element.find('.lnkt-table-control-left'),
        table: element.find('.lnkt-table'),
        legend: element.find('.lnkt-table-legend'),
        totalRecord: element.find('.lnkt-table-total-records'),
        pagingEntry: element.find('.lnkt-table-paging-entry'),
        filterInfo: element.find('.lnkt-table-filter-info'),
        previousButton: element.find('.lnkt-table-previous'),
        nextButton: element.find('.lnkt-table-next')
      },
      options,
      fetchData,
      clearAll,
      setIsLoading,
      renderFooterFilter,
      initFilter,
      updateFilter
    };
  }

  $.fn.lnktTable = function (options) {
    var instance = $(this);
    var data = instance.data();
    if (data && data.isLnktTable) {
      return data;
    } else {
      options = Object.assign({
        noResultText: 'No results found',
        dataItems: [],
        totalRecords: 0,
        pagination: {
          sortBy: '',
          sortDirection: '',
          pageIndex: 0,
          pageSize: 10
        },
        pageSizes: [10, 25, 50, 100],
        searchValue: '',
        filters: {},
        dropDownFilters: {},
        allowSelecting: false,
        disable: false
      }, options);
      var tableInstance = createTable(this, options);
      instance.data(tableInstance);
      options.onTableCreated && options.onTableCreated.apply(tableInstance);
      options.fetchOnCreated && tableInstance.fetchData();
      options.fetchOnCreated && tableInstance.initFilter();
      return tableInstance;
    }
  };

  $.fn.lnktTable.version = "1.0.0";
  $.fn.lnktTable.debounceTime = 500;
})(window, jQuery);
