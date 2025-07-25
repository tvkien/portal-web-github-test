function StudentPreferenceUtil() { };

StudentPreferenceUtil.IsNullOrEmpty = function (value) {
    return typeof (value) === "undefined" || value == null || $.trim(value) == '';
};

StudentPreferenceUtil.ParseInt = function (value) {
    if (typeof (value) === "undefined" || value == null || $.trim(value) == '') return 0;
    return parseInt(value);
};

function initPreferenceNames (districtLabel) {
  return preferenceNames = [
    { name: 'Show Test', key: 'showTest' },
    { name: 'Visibility In Test-Specific Options', key: 'visibilityInTestSpecific' },
    { name: 'Show Notes', key: 'showNotes' },
    { name: 'Show Artifacts', key: 'showArtifacts' },
    { name: 'Show Associated Questions', key: 'showAssociatedQuestions' },
    { name: 'Show Standards', key: 'showStandards' },
    { name: 'Show Topics', key: 'showTopics' },
    { name: 'Show Skills', key: 'showSkills' },
    { name: 'Show Custom Tags', key: 'showCustomTags' },
    { name: 'Show Other Tags', key: 'showOtherTags' },
    { name: 'Show Item Data', key: 'showItemData' },
    { name: 'Show Question Content', key: 'showQuestions' },
    { name: 'Show Correct Answers', key: 'showCorrectAnswers' },
    { name: 'Show Student Answers', key: 'showStudentAnswers' },
    { name: 'Show Points Possible', key: 'showPointsPossible' },
    { name: 'Show Student Time Spent', key: 'showTimeSpent' },
    { name: 'Show Class Time Spent (Avg.)', key: 'showTimeSpentClass' },
    { name: 'Show School Time Spent (Avg.)', key: 'showTimeSpentSchool' },
    { name: 'Show ' + districtLabel + ' Time Spent (Avg.)', key: 'showTimeSpentDistrict' },
    { name: 'Show Class Averages', key: 'showClassAverages' },
    { name: 'Show School Averages', key: 'showSchoolAverage' },
    { name: 'Show ' + districtLabel + ' Averages', key: 'showDistrictAverage' },
    { name: 'Can Review Test', key: 'reviewTest' },
    { name: 'Show Teacher Comment', key: 'showTeacherComment' },
    { name: 'Show Summary Detail', key: 'showSummaryDetail' },
    { name: 'Launch Item Analysis', key: 'launchItemAnalysis' },
    { name: 'Show Item Detail Chart', key: 'showItemDetailChart' },
    { name: 'Show School Percentile', key: 'showSchoolPercentile' },
    { name: 'Show District Percentile', key: 'showDistrictPercentile' }
  ];
}

