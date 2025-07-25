/**
 * Registers shapes
 */
(function () {

	function SingleArrowShape () {
		mxActor.call(this);
	}

	mxUtils.extend(SingleArrowShape, mxActor);
	SingleArrowShape.prototype.arrowWidth = 0.3;
	SingleArrowShape.prototype.arrowSize = 0.2;
	SingleArrowShape.prototype.redrawPath = function(c, x, y, w, h) {
		var aw = h * Math.max(0, Math.min(1, parseFloat(mxUtils.getValue(this.style, 'arrowWidth', this.arrowWidth))));
		var as = w * Math.max(0, Math.min(1, parseFloat(mxUtils.getValue(this.style, 'arrowSize', this.arrowSize))));
		var at = (h - aw) / 2;
		var ab = at + aw;

		var arcSize = mxUtils.getValue(this.style, mxConstants.STYLE_ARCSIZE, mxConstants.LINE_ARCSIZE) / 2;
		this.addPoints(c, [new mxPoint(0, at), new mxPoint(w - as, at), new mxPoint(w - as, 0), new mxPoint(w, h / 2),
		                   new mxPoint(w - as, h), new mxPoint(w - as, ab), new mxPoint(0, ab)],
		                   this.isRounded, arcSize, true);
		c.end();
	};

	mxCellRenderer.prototype.defaultShapes['singleArrow'] = SingleArrowShape;

	function DoubleArrowShape () {
		mxActor.call(this);
	}

	mxUtils.extend(DoubleArrowShape, mxActor);

	DoubleArrowShape.prototype.redrawPath = function(c, x, y, w, h) {
		var aw = h * Math.max(0, Math.min(1, parseFloat(mxUtils.getValue(this.style, 'arrowWidth', SingleArrowShape.prototype.arrowWidth))));
		var as = w * Math.max(0, Math.min(1, parseFloat(mxUtils.getValue(this.style, 'arrowSize', SingleArrowShape.prototype.arrowSize))));
		var at = (h - aw) / 2;
		var ab = at + aw;

		var arcSize = mxUtils.getValue(this.style, mxConstants.STYLE_ARCSIZE, mxConstants.LINE_ARCSIZE) / 2;
		this.addPoints(c, [new mxPoint(0, h / 2), new mxPoint(as, 0), new mxPoint(as, at), new mxPoint(w - as, at),
		                   new mxPoint(w - as, 0), new mxPoint(w, h / 2), new mxPoint(w - as, h),
		                   new mxPoint(w - as, ab), new mxPoint(as, ab), new mxPoint(as, h)],
		                   this.isRounded, arcSize, true);
		c.end();
	};

	mxCellRenderer.prototype.defaultShapes['doubleArrow'] = DoubleArrowShape;
})();
