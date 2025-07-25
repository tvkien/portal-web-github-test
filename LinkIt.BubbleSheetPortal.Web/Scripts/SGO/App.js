var SGOService = {
    archive: function (sgoId) {
        return $.post('/SGOManage/ArchiveSGO', { sgoId: sgoId });
    }
};

var SGOViewModel = new Vue({
    el: 'body',
    data: {
        isShowModalArchive: false,
        isShowModalUnarchive: false,
        sgoButton: false,
        sgoError: '',
        sgoId: 0
    },
    methods: {
        acceptArchive: function () {
            this.toggleArchive();
        },
        closeModalArchive: function () {
            this.isShowModalArchive = false;
        },
        acceptUnarchive: function () {
            this.toggleArchive();
        },
        closeModalUnarchive: function () {
            this.isShowModalUnarchive = false;
        },
        toggleArchive: function () {
            var self = this;

            self.sgoButton = true;

            SGOService.archive(self.sgoId).done(function (response) {
                self.sgoButton = false;

                if (response.Success) {
                    ui.dataTable.fnDraw(false);
                    self.isShowModalArchive = false;
                    self.isShowModalUnarchive = false;
                    self.sgoError = '';
                } else {
                    self.sgoError = 'Have a Error, Please try again.';
                }
            }).error(function (err) {
                self.sgoButton = false;
                self.sgoError = 'Have a Error, Please try again.';
            });
        }
    }
});
