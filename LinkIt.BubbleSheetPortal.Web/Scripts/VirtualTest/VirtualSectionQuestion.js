var VirtualSectionQuestionService = {
    deleteQuestionGroup: function (params) {
        return $.ajax({
            type: 'POST',
            url: '/VirtualTest/DeleteQuestionGroup',
            data: params
        });
    }
};

var VirtualSectionQuestionModel = new Vue({
    el: '#virtualSectionQuestion',
    data: {
        isShowModalDeleteGroup: false,
        virtualSectionQuestion: {
            virtualTestId: 0,
            questiongroupId: 0
        }
    },
    methods: {
        closeModalDeleteGroup: function () {
            this.isShowModalDeleteGroup = false;
        },
        deleteQuestionGroup: function (isDeleteOnlyQuestionGroup) {
            var self = this;
            var $divMain = $('#divMain');
            var params = {
                virtualTestId: self.virtualSectionQuestion.virtualTestId,
                questiongroupId: self.virtualSectionQuestion.questiongroupId,
                isDeleteOnlyQuestionGroup: isDeleteOnlyQuestionGroup
            };

            $('.virtualGroup.selected').removeClass('selected')
            groupHeaderSelected = null;
            self.isShowModalDeleteGroup = false;
            window.isClickQuestionGroup = true;
            ShowBlock($divMain, 'Loading');

            VirtualSectionQuestionService.deleteQuestionGroup(params).success(function (response) {
                if (response.success) {
                    RefreshSectionQuestion(self.virtualSectionQuestion.virtualTestId);
                    showPropertiesVirtualTest();
                } else {
                    CustomAlert(response.ErrorMessage,true);
                    $divMain.unblock();
                }

            }).error(function (err) {
                console.log(err);
            });
        }
    }
});