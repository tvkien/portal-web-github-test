(function($) {
    'use strict';

    // Get height max assignment note
    getHeightAssignmentNote();

    // Select custom filter student
    $('#assignment-filter-student').select2({
        minimumResultsForSearch: Infinity,
        containerCssClass: 'assignment-select',
        dropdownCssClass: 'assignment-select-dropdown'
    });

    // Select custom select student
    $('#assignment-list-student').select2({
        minimumResultsForSearch: Infinity,
        templateResult: formatListStudent,
        templateSelection: formatListStudent,
        width: '300',
        containerCssClass: 'assignment-select',
        dropdownCssClass: 'assignment-select-dropdown'
    });

    function formatListStudent(student) {
        var $student;
        var studentStatus = $(student.element).data('status');
        var studentHtml = '';

        if (!student.id) { return student.text; }

        studentHtml += '<div class="assignment-list-student">';
        studentHtml += '<span class="assignment-list-student-icon"><span class="icon icon-' + studentStatus + '"></span></span>';
        studentHtml += '<span class="assignment-list-student-name">' + student.text + '</span></div>';
        studentHtml += '</div>';

        $student = $(studentHtml);

        return $student;
    }

    function getHeightAssignmentNote() {
        var maxHeightAssignNote = 0;
        var arrAssignNote = [];

        $('.assignment-note').each(function(ind, assignnote) {
            var $assignnote = $(assignnote);

            arrAssignNote.push($assignnote.height());
        });

        maxHeightAssignNote = Math.max.apply(null, arrAssignNote);

        $('.assignment-note').css('height', maxHeightAssignNote + 'px');
    }
}(jQuery));
