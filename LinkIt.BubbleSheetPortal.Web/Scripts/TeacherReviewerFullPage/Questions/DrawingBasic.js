var MX_GRAPH_IMAGES = 'Scripts/mxgraph/editors/images/';
var MX_GRAPH_ADD_WIDTH = 52;
var MX_GRAPH_ADD_HEIGHT = 24;
var MX_GRAPH_INTERVAL = 100;
var MX_NO_DRAW = 'bgdraw.png';

(function ($) {
    $.widget('jquery.DrawingBasic', {
        options: {
            DrawingBasicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },
        
        ShowGraph: function() {
            var that = this;
            var $assignmentDescQuestion = $('.assignment-desc-question');
            var extendedTextInteraction = $assignmentDescQuestion.find('extendedTextInteraction[data-type="basic"]');
            
            if (extendedTextInteraction.length) {
                extendedTextInteraction.each(function (ind, graphAndLabel) {
                    var $graphAndLabel = $(graphAndLabel);

                    that.LoadDrawingDetail($graphAndLabel, ind);

                    if ($graphAndLabel.find('.divdraw').find('.red-border').length) {
                        var graphAndLabelRespondId = $graphAndLabel.attr('responseidentifier');
                        var graphAndLabelWidth = $graphAndLabel.attr('data-width');

                        $graphAndLabel
                            .find('.newContainer' + graphAndLabelRespondId)
                            .addClass('red-border')
                            .width(graphAndLabelWidth);
                    }

                    $graphAndLabel.find('.divdraw').remove();
                });
            }
        },

        UpdateDrawQuestion: function(graph, questionDrawingContent, index) {
            var that = this;

            if (graph && questionDrawingContent) {
                var questionData = questionDrawingContent.Data;
                var newArr = [];
                for (var i = 0; i < questionData.length; i++) {
                    if(questionData[i].Xml != '') {
                        newArr.push(questionData[i]);
                    }
                }
                var questionItem = newArr[index];
                if (graph && questionItem.Xml !== '') {
                    that.LoadXml(graph, questionItem.Xml);
                }
            }
        },
        
        CreateEditor: function (container, opts) {
            var that = this;
            var graph = null;
        
            try {
                // Checks if browser is supported
                if (!mxClient.isBrowserSupported()) {
                    // Displays an error message if the browser is not supported.
                    mxUtils.error('Browser is not supported!', 200, false);
                } else {
                    var isImage = true;
        
                    if (opts.src.indexOf('/Content/images/emptybg.png') > -1 || opts.src == '' || opts.src == null) {
                        isImage = false;
                    }
        
                    var isDrawingSize = opts.drawingWidth || opts.drawingHeight;
        
                    mxConstants.DEFAULT_FONTFAMILY = 'Helvetica';
                    mxConstants.DEFAULT_FONTSIZE = 14;
        
                    // Sets the global shadow color
                    mxConstants.SHADOWCOLOR = '#c0c0c0';
                    mxConstants.SHADOW_OPACITY = 0.5;
                    mxConstants.SHADOW_OFFSET_X = 4;
                    mxConstants.SHADOW_OFFSET_Y = 4;
                    mxConstants.HANDLE_FILLCOLOR = '#99ccff';
                    mxConstants.HANDLE_STROKECOLOR = '#0088cf';
                    mxConstants.VERTEX_SELECTION_COLOR = '#00a8ff';
                    mxConstants.HIGHLIGHT_OPACITY = 0;
        
                    // Enables connections along the outline
                    mxConnectionHandler.prototype.outlineConnect = true;
                    mxEdgeHandler.prototype.manageLabelHandle = true;
                    mxEdgeHandler.prototype.outlineConnect = true;
                    mxCellHighlight.prototype.keepOnTop = true;
        
                    // Enable rotation handle and live preview
                    mxVertexHandler.prototype.rotationEnabled = true;
                    mxVertexHandler.prototype.manageSizers = true;
                    mxVertexHandler.prototype.livePreview = true;
        
                    // Overridden to define per-shape connection points
                    mxGraph.prototype.getAllConnectionConstraints = function (terminal, source) {
                        if (terminal != null && terminal.shape != null) {
                            if (terminal.shape.stencil != null) {
                                return terminal.shape.stencil.constraints;
                            } else if (terminal.shape.constraints != null) {
                                return terminal.shape.constraints;
                            }
                        }
        
                        return null;
                    };
        
                    // Defines the default constraints for all shapes
                    mxShape.prototype.constraints = [
                        new mxConnectionConstraint(new mxPoint(0.25, 0), true),
                        new mxConnectionConstraint(new mxPoint(0.5, 0), true),
                        new mxConnectionConstraint(new mxPoint(0.75, 0), true),
                        new mxConnectionConstraint(new mxPoint(0, 0.25), true),
                        new mxConnectionConstraint(new mxPoint(0, 0.5), true),
                        new mxConnectionConstraint(new mxPoint(0, 0.75), true),
                        new mxConnectionConstraint(new mxPoint(1, 0.25), true),
                        new mxConnectionConstraint(new mxPoint(1, 0.5), true),
                        new mxConnectionConstraint(new mxPoint(1, 0.75), true),
                        new mxConnectionConstraint(new mxPoint(0.25, 1), true),
                        new mxConnectionConstraint(new mxPoint(0.5, 1), true),
                        new mxConnectionConstraint(new mxPoint(0.75, 1), true)
                    ];
        
                    // Edges have no connection points
                    mxPolyline.prototype.constraints = null;
                    mxHexagon.prototype.constraints = null;
        
                    // Creates the div for the graph
                    var gridSize = opts.gridSize;
                    var gridContainer = that.CreateGrid(gridSize, opts);
                    container.appendChild(gridContainer);
        
                    // Workaround for Internet Explorer ignoring certain styles
                    if (mxClient.IS_QUIRKS) {
                        document.body.style.overflow = 'hidden';
                        new mxDivResizer(gridContainer);
                    }
        
                    // Creates the model and the graph inside the container
                    // using the fastest rendering available on the browser
                    var model = new mxGraphModel();
        
                    graph = new mxGraph(gridContainer, model);
                    graph.dropEnabled = true;
        
                    if (isImage) {
                        graph.setBackgroundImage(new mxImage(opts.src, opts.width, opts.height));
        
                        if (isDrawingSize) {
                            graph.maximumGraphBounds = new mxRectangle(0, 0, opts.drawingWidth, opts.drawingHeight);
                        } else {
                            graph.maximumGraphBounds = new mxRectangle(0, 0, opts.width, opts.height);
                        }
                    }
        
                    graph.minimumGraphSize = new mxRectangle(0, 0, opts.width, opts.height);
                    graph.setResizeContainer(false);
        
                    // Matches DnD inside the graph
                    mxDragSource.prototype.getDropTarget = function (graph, x, y) {
                        var cell = graph.getCellAt(x, y);
        
                        if (!graph.isValidDropTarget(cell)) {
                            cell = null;
                        }
        
                        return cell;
                    };
        
                    // Enables new connections in the graph
                    graph.setConnectable(true);
                    graph.setMultigraph(true);
                    graph.zoomOut();
                    graph.zoomActual();
        
                    // Enables connect preview for the default edge style
                    graph.connectionHandler.createEdgeState = function () {
                        var edge = graph.createEdge(null, null, null, null, null);
                        return new mxCellState(this.graph.view, edge, this.graph.getCellStyle(edge));
                    };
        
                    // Specifies the default edge style
                    var graphStyle = graph.getStylesheet().getDefaultEdgeStyle();
                    graphStyle['edgeStyle'] = 'loopEdgeStyle';
        
                    that.CreateActions(container, graph, isImage);
        
                    // Stops editing on enter or escape keypress
                    new mxRubberband(graph);
                    new mxKeyHandler(graph);
                    mxObjectCodec.allowEval = true;
        
                    graph.setEnabled(false);
        
                    if (isImage) {
                        var parent = graph.getDefaultParent();
        
                        // Adds cells to the model in a single step
                        graph.getModel().beginUpdate();
                        try {
                            graph.insertVertex(parent, null, '', 0, 0, 1, 1, 'fillColor=transparent;strokeColor=none;pointerEvents=none;opacity=0');
        
                            if (isDrawingSize) {
                                graph.insertVertex(parent, null, '', opts.drawingWidth, opts.drawingHeight, 1, 1, 'fillColor=transparent;strokeColor=none;pointerEvents=none;opacity=0');
                            } else {
                                graph.insertVertex(parent, null, '', opts.width, opts.height, 1, 1, 'fillColor=transparent;strokeColor=none;pointerEvents=none;opacity=0');
                            }
                        } finally {
                            // Updates the display
                            graph.getModel().endUpdate();
                        }
                    }
                }
            } catch (e) {
                mxUtils.alert('Cannot start application: ' + e.message);
            }
        
            return graph;
        },
        
        CreateGrid: function(gridSize, opts) {
            var grid = document.createElement('div');
        
            if (gridSize == null) {
                gridSize = 'x1';
            }
            
            grid.style.width = (parseInt(opts.drawingWidth, 10) + 1) + 'px';
            grid.style.height = (parseInt(opts.drawingHeight, 10) + 1) + 'px';
            grid.style.position = 'relative';
            grid.style.overflow = 'auto';
            grid.style.background = 'url("' + MX_GRAPH_IMAGES + gridSize + '.gif")';

            return grid;
        },
        
        CreateActions: function(container, graph, isImage) {
            var actionsContainer = document.createElement('div');
        
            actionsContainer.className = 'mxActionsMode';
            actionsContainer.style.backgroundColor = '#fff';
            actionsContainer.style.position = 'relative';
            actionsContainer.style.fontSize = '12px';
            actionsContainer.style.height = '24px';
            actionsContainer.style.lineHeight = '24px';
            actionsContainer.style.textIndent = '4px';
            actionsContainer.style.width = '100%';
        
            if (!isImage) {
                mxUtils.write(actionsContainer, 'Zoom: ');
        
                actionsContainer.appendChild(mxUtils.button('In', function () {
                    graph.zoomIn();
                }));
        
                mxUtils.write(actionsContainer, ', ');
        
                actionsContainer.appendChild(mxUtils.button('Out', function () {
                    graph.zoomOut();
                }));
        
                mxUtils.write(actionsContainer, ', ');
        
                actionsContainer.appendChild(mxUtils.button('Actual', function () {
                    graph.zoomActual();
                }));
        
                mxUtils.write(actionsContainer, ', ');
        
                actionsContainer.appendChild(mxUtils.button('Fit', function () {
                    graph.fit();
                }));
        
                mxUtils.write(actionsContainer, ' ');
            }
        
            container.appendChild(actionsContainer);
        },
        
        LoadXml: function(graph, xml) {
            graph.getModel().beginUpdate();
            try {
                var xmlDocument = mxUtils.parseXml(xml);
                var node = xmlDocument.documentElement;
                var decoder = new mxCodec(xmlDocument);
                decoder.decode(node, graph.getModel());
            } finally {
                // Updates the display
                graph.getModel().endUpdate();
            }
        },
        
        LoadDrawingDetail: function(container, index) {
            var that = this;
            var responseId = container.attr('responseidentifier');
            var $newContainer = $('.newContainer' + responseId);
        
            if ($newContainer.length > 0) {
                $newContainer.remove();
            }
        
            var newContainer = document.createElement('div');
            newContainer.className = 'newContainer' + responseId;
        
            container.append(newContainer);
            
            var $img = container.find('img');
        
            var opts = {
                drawingHeight: container.attr('data-height'),
                drawingWidth: container.attr('data-width'),
                height: $img.attr('height'),
                width: $img.attr('width'),
                gridSize: container.attr('gridSize'),
                src: $img.attr('src')
            };
        
            var graph = that.CreateEditor(newContainer, opts);
            
            if(!viewModel.DrawingContent()) {
                return;
            }
        
            var questionDrawingContent = viewModel.DrawingContent();
            questionDrawingContent = JSON.parse(questionDrawingContent);
            
            that.UpdateDrawQuestion(graph, questionDrawingContent[0] , index);
        }
    });
}(jQuery));