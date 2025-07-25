var BubbleSheetReviewDetails = new Vue({
    el: '[vue="bubbleSheetReviewDetails"]',
    data: {
        isShowBubbleSheetImage: false,
        bubbleSheetImageHtml: ''
    },
    methods: {
        toggleBubbleSheetImageModal: function () {
            this.isShowBubbleSheetImage = !this.isShowBubbleSheetImage;
            $('.modal-component-draggable-resizable').removeAttr('data-x data-y');
        },
        getBubbleSheetReviewImage: function (dataUrl) {
            var imageContainer = document.createElement('div');
            var imageWrapper = document.createElement('div');
            var image = document.createElement('img')
    
            imageContainer.className = 'bubbleSheetClassReviewImage';
            imageWrapper.className = 'bubbleSheetClassReviewImage__item u-inline-block';
    
            image.src = dataUrl;
            image.setAttribute('alt', '');
    
            imageWrapper.appendChild(image);
            imageContainer.appendChild(imageWrapper);
    
            return imageContainer;
        }
    }
});
