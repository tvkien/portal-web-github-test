var StudentEnterCtrl = {
    deleteAutoSave: function (params) {
        return $.ajax({
            type: 'POST',
            url: '/DataLockerEntryResult/DeleteAutoSaveData/',
            data: params
        });
    }
};


var StudentEnterModel = new Vue({
    el: '#generateContent',
    data: {        
        isShowModalWarning: false
    },
    methods: {
        loadDataFromAutoSave: function(fromAutoSave) {
            this.isShowModalWarning = false;
            if (!fromAutoSave) {
                this.deleteAutoSave();                
            } else {
                redirectToEntryResult();
            }
        },
        deleteAutoSave: function () {
            var params = { virtualTestId: $('#selectTest').val(), classId: $('#selectClass').val() };
            StudentEnterCtrl.deleteAutoSave(params)
                .done(function() {
                    redirectToEntryResult();
                });
        }
    }
});
