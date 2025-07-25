function QuestionFilter(data) {
    var self = this;

    self.Id = ko.observable(data.Id);
    self.Value = ko.observable(data.Value);
    self.IconClasses = ko.observable(data.IconClasses);
    self.Text = ko.observable(data.Text);
}
