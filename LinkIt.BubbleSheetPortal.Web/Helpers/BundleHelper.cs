using SquishIt.Framework;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{

    [AttributeUsage(AttributeTargets.Method)]
    public class BundleAppStartMethodAttribute : Attribute
    {

    }

    public class BundleHelper
    {
        #region Account
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptAccountLogOnBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                  .Add("/Scripts/jquery.dataTables-1.9.4.js")
                  .Add("/Scripts/custom.js")
                  .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Render("/Content/combined/script/LogOn_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptAccountLogOnBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                  .Add("/Scripts/jquery.dataTables-1.9.4.js")
                  .Add("/Scripts/customV2.js")
                  .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Render("/Content/combined/script/LogOn_#.js")
                );
        }
        #endregion

        #region ACTReport
        [BundleAppStartMethod]
        public static MvcHtmlString StyleACTReportReportPrintingBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                     .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/standard.css")
                    .Add("~/Content/themes/Constellation/css/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/css/prettyPhoto.css")
                    .Add("~/Content/css/utilitySearch.css")
                    .Render("/Content/combined/css/ACTReport_#.css")
                );
        }
        #endregion

        #region Admin
        [BundleAppStartMethod]
        public static MvcHtmlString StyleAdminDistrictBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/calendars.css")
                .Add("~/Content/themes/Constellation/css/960.gs.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/themes/Constellation/css/block-lists.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Add("~/Content/themes/Constellation/css/wizard.css")
                .Add("~/Content/themes/Constellation/css/gallery.css")
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                    .Render("/Content/combined/css/AdminDistrict_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StylePurgeTestBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                    .Render("/Content/combined/css/PurgeTest_#.css")
                );
        }

        #endregion

        #region AnswerViewer
        [BundleAppStartMethod]
        public static MvcHtmlString StyleAnswerViewerBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("/Content/themes/AnswerViewer/css/base/normalize.css")
                .Add("/Content/themes/AnswerViewer/css/answerviewer.css")
                .Add("/Content/themes/AnswerViewer/css/modules/generic.css")
                .Add("/Content/themes/AnswerViewer/css/modules/table.css")
                .Add("/Content/themes/AnswerViewer/css/components/multiplechoice.css")
                .Add("/Content/themes/AnswerViewer/css/components/inlinechoice.css")
                .Add("/Content/themes/AnswerViewer/css/components/textentry.css")
                .Add("/Content/themes/AnswerViewer/css/components/extendedtext.css")
                .Add("/Content/themes/AnswerViewer/css/components/dragdropstandard.css")
                .Add("/Content/themes/AnswerViewer/css/components/texthotspot.css")
                .Add("/Content/themes/AnswerViewer/css/components/imagehotspot.css")
                .Add("/Content/themes/AnswerViewer/css/components/tablehotspot.css")
                .Add("/Content/themes/AnswerViewer/css/components/numberlinehotspot.css")
                .Add("/Content/themes/AnswerViewer/css/components/dragdropsequence.css")
                .Add("/Content/themes/AnswerViewer/css/components/guidance-rationale.css")
                .Add("/Content/themes/AnswerViewer/css/components/modal.css")
                .Add("/Content/themes/AnswerViewer/css/components/glossary.css")
                .Add("/Content/themes/AssignmentRegrader/css/tooltipster.css")
                    .Render("/Content/combined/css/AnswerViewer_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptAnswerViewerBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/jquery-1.7.1.min.js")
                .Add("/Scripts/Lib/ramda.min.js")
                .Add("/Scripts/Lib/vue.min.js")
                .Add("/Scripts/Lib/vue-resource.min.js")
                .Add("/Scripts/Lib/vue-modal.js")
                .Add("/Content/themes/TestMaker/mediaelement.min.js")
                .Add("/Content/themes/TestMaker/jquery.tooltipster.min.js")
                .Add("/Scripts/AnswerViewer/Utils/Utils.js")
                .Add("/Scripts/AnswerViewer/Service/AnswerViewerService.js")
                .Add("/Scripts/AnswerViewer/Questions/Multipart.js")
                .Add("/Scripts/AnswerViewer/Questions/MultipleChoice.js")
                .Add("/Scripts/AnswerViewer/Questions/MultipleChoiceVariable.js")
                .Add("/Scripts/AnswerViewer/Questions/InlineChoice.js")
                .Add("/Scripts/AnswerViewer/Questions/TextEntry.js")
                .Add("/Scripts/AnswerViewer/Questions/ExtendedText.js")
                .Add("/Scripts/AnswerViewer/Questions/DrawInteraction.js")
                .Add("/Scripts/AnswerViewer/Questions/DragDropStandard.js")
                .Add("/Scripts/AnswerViewer/Questions/DragDropNumerical.js")
                .Add("/Scripts/AnswerViewer/Questions/TextHotSpot.js")
                .Add("/Scripts/AnswerViewer/Questions/ImageHotSpot.js")
                .Add("/Scripts/AnswerViewer/Questions/TableHotSpot.js")
                .Add("/Scripts/AnswerViewer/Questions/NumberLineHotSpot.js")
                .Add("/Scripts/AnswerViewer/Questions/DragDropSequence.js")
                .Add("/Scripts/AnswerViewer/Others/Video.js")
                .Add("/Scripts/AnswerViewer/Others/Guidance.js")
                .Add("/Scripts/AnswerViewer/Others/Glossary.js")
                .Add("/Scripts/AnswerViewer/Question.js")
                .Add("/Scripts/AnswerViewer/App.js")
                    .Render("/Content/combined/script/AnswerViewer_#.js")
                );
        }

        #endregion

        #region DataLocker
        //DataLocker/Index
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptDataLockerBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                  .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                  .Add("/Scripts/custom.js")
                    .Add("/Scripts/Qtip/jquery.qtip.js")
                    .Render("/Content/combined/script/DataLocker_#.js")
                );
        }

        //DataLocker/Template
        [BundleAppStartMethod]
        public static MvcHtmlString StyleDataLockerTemplateBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                .Add("~/Scripts/Qtip/jquery.qtip.css")
                .Render("/Content/combined/css/DataLockerTemplate_#.css")
                );
        }
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptViewDataLockerTemplateBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("~/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                .Add("/Content/themes/AssignmentRegrader/js/css_browser_selector.js")
                .Add("~/Scripts/jquery.validate.min.js")
                .Add("~/Scripts/jquery.validate.unobtrusive.min.js")
                .Add("~/Scripts/jquery.numeric.min.js")
                .Add("~/Content/themes/TestMaker/ckeditor_utils.js")
                .Render("/Content/combined/script/ViewTemplate_#.js")
            );
        }

        #endregion

        #region DataLockerEntryResult
        [BundleAppStartMethod]
        public static MvcHtmlString StyleDataLockerEntryResultBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Scripts/Qtip/jquery.qtip.css")
                    .Add("~/Scripts/Lib/handsontable/pro/v3/handsontable.full.min.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Add("~/Content/css/vue-components/vue-student-entry.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/select2.css")
                    .Render("/Content/combined/css/DataLockerEntryResult_#.css")
            );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptDataLockerEntryResultBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/moment.min.js")
                    .Add("/Scripts/Qtip/jquery.qtip.js")
                    .Add("/Scripts/Lib/handsontable/pro/v3/handsontable.full.min.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Add("/Scripts/jquery.listSplitter.js")
                    .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                    .Add("/Scripts/custom.js")
                    .Add("~/Scripts/select2.full.min.js")
                    .Add("/Scripts/Lib/vue-select2.js")
                    .Add("/Scripts/Lib/lazy-component.js")
                    .Render("/Content/combined/script/DataLockerEntryResult_#.js")
            );
        }
        [BundleAppStartMethod]
        public static MvcHtmlString StyleDataLockerMultiDateEntryResultBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Scripts/Qtip/jquery.qtip.css")
                    .Add("~/Scripts/Lib/handsontable/pro/v3/handsontable.full.min.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Add("~/Content/css/vue-components/vue-student-entry.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/select2.css")
                    .Render("/Content/combined/css/DataLockerMultiDateEntryResult_#.css")
            );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptDataLockerMultiDateEntryResultBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/moment.min.js")
                    .Add("/Scripts/Qtip/jquery.qtip.js")
                    .Add("/Scripts/Lib/handsontable/pro/v3/handsontable.full.min.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Add("/Scripts/jquery.listSplitter.js")
                    .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                    .Add("/Scripts/custom.js")
                    .Add("~/Scripts/select2.full.min.js")
                    .Render("/Content/combined/script/DataLockerMultiDateEntryResult_#.js")
            );
        }
        #endregion

        #region HelpResource
        [BundleAppStartMethod]
        public static MvcHtmlString StyleHelpResourceBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                     .Add("~/Content/themes/HelpResource/HelpResourceCss.css")
                    .Render("/Content/combined/css/HelpResource_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptHelpResourceBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/knockout-3.4.2.js")
                .Add("/Scripts/HelpResource/ShowDiaglog.js")
                .Add("/Scripts/HelpResource/KnockModel.js")
                .Add("/Scripts/HelpResource/SearchResultCallBackPublish.js")
                .Add("/Scripts/HelpResource/SearchResultPublish.js")
                    .Render("/Content/combined/script/HelpResource_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptHelpResourceAdminBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/knockout-3.4.2.js")
                .Add("/Scripts/HelpResource/ShowDiaglog.js")
                .Add("/Scripts/HelpResource/KnockModel.js")
                .Add("/Scripts/HelpResource/SearchResultCallBackAdmin.js")
                .Add("/Scripts/HelpResource/SearchResultAdmin.js")
                    .Render("/Content/combined/script/HelpResource_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptHelpResourceAdminBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/knockout-3.4.2.js")
                .Add("/Scripts/HelpResource/ShowDiaglog.js")
                .Add("/Scripts/HelpResource/KnockModel.js")
                .Add("/Scripts/HelpResource/v2/SearchResultCallBackAdmin.js")
                .Add("/Scripts/HelpResource/SearchResultAdmin.js")
                    .Render("/Content/combined/script/HelpResource_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptHelpResourceUploadBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/knockout-3.4.2.js")
                .Add("/Scripts/HelpResource/ShowDiaglog.js")
                .Add("/Scripts/HelpResourceUpload/KnockModel.js")
                    .Render("/Content/combined/script/HelpResourceUpload_#.js")
                );
        }

        #endregion

        #region ItemBank
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptItembankItemsFromLibraryBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                    .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                    .Render("/Content/combined/script/ItembankItemsFromLibrary_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptItembankItemsFromLibraryBundle2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/imagesloaded.pkgd.js")
                    .Add("/Scripts/PrintTest/PrintTest.js")
                    .Render("/Content/combined/script/ItembankItemsFromLibrary_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleItemsFromLibraryBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                .Add("~/Content/themes/TestMaker/contents.css")
                .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                .Add("~/Content/themes/TestDesign/TreeView.css")
                .Add("~/Scripts/Texttospeech/texttospeech.css")
                    .Render("/Content/combined/css/ItemsFromLibrary_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleItemsFromLibraryBundle2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/themes/AssessmentItem/css/popup.css")
                .Add("~/Content/themes/LinkitStyleSheet.css")
                .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                .Add("~/Content/themes/TestDesign/TestDesign.css")
                .Add("~/Content/css/import-item.css")
                    .Render("/Content/combined/css/ItemsFromLibrary_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleItemsFromLibraryBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/themes/AssessmentItem/css/popup-v2.css")
                .Add("~/Content/themes/LinkitStyleSheet.css")
                .Add("~/Content/css/v2/ItemSet-v2.css")
                .Add("~/Content/themes/TestDesign/TestDesign.css")
                .Add("~/Content/css/import-item.css")
                    .Render("/Content/combined/css/ItemsFromLibrary_#.css")
                );
        }
        #endregion

        #region KnowsysReport
        [BundleAppStartMethod]
        public static MvcHtmlString StyleReportPrintingMultipleStudentsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
               .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                    .Render("/Content/combined/css/ReportPrinting_#.css")
                );
        }
        #endregion

        #region MonitoringTestTaking
        [BundleAppStartMethod]
        public static MvcHtmlString StyleProctorTestViewBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                   .Add("~/Content/css/custom.css")
                   .Add("~/Content/css/linkitStyleSheet.css")
                   .Add("~/Content/css/linkitMonitoringTestTaking.css")
                    .Render("/Content/combined/css/ProctorTestView_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptProctorTestViewBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Render("/Content/combined/script/ProctorTestView_#.js")
                );
        }
        #endregion

        #region PassageEditor
        [BundleAppStartMethod]
        public static MvcHtmlString StylePassageEditorBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                    .Add("~/Content/themes/TestMaker/contents.css")
                    .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Render("/Content/combined/css/PassageEditor_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptPassageEditorBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                    .Render("/Content/combined/script/PassageEditor_#.js")
                );
        }

        #endregion

        #region PassThrough
        [BundleAppStartMethod]
        public static MvcHtmlString StylePassThroughBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/calendars.css")
                .Add("~/Content/themes/Constellation/css/960.gs.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/themes/Constellation/css/block-lists.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Add("~/Content/themes/Constellation/css/wizard.css")
                .Add("~/Content/themes/Constellation/css/gallery.css")
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                    .Render("/Content/combined/css/PassageThrough_#.css")
                );
        }
        #endregion

        #region QTIItem
        [BundleAppStartMethod]
        public static MvcHtmlString StyleQTIItemTestMakerBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                    .Add("~/Content/themes/TestMaker/contents.css")
                    .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Add("~/Scripts/Texttospeech/texttospeech.css")
                    .Render("/Content/combined/css/QTIItem_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleQTIItemAssessmentBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                        .Add("~/Content/themes/AssessmentItem/css/popup.css")
                        .Add("~/Content/themes/LinkitStyleSheet.css")
                        .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                         .Render("/Content/combined/css/QTIItem_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptQTIItemBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                    .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                    .Render("/Content/combined/script/QTIItem_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptQTIItemBundle2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/imagesloaded.pkgd.js")
                    .Add("/Scripts/PrintTest/PrintTest.js")
                    .Render("/Content/combined/script/QTIItem_combined_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleQTIItemPassageDetailBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                    .Add("~/Content/themes/LinkitStyleSheet.css")
                    .Render("/Content/combined/css/QTIItemPassageDetail_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleQTIItemVirtualTestIndexBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/AssessmentItem/css/popup.css")
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/form.css")
                    .Render("/Content/combined/css/QTIItemVirtualTest_#.css")
                );
        }
        #endregion

        #region Registration
        [BundleAppStartMethod]
        public static MvcHtmlString StyleRegistrationBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/calendars.css")
                .Add("~/Content/themes/Constellation/css/960.gs.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/themes/Constellation/css/block-lists.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Add("~/Content/themes/Constellation/css/wizard.css")
                .Add("~/Content/themes/Constellation/css/gallery.css")
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                .Add("~/Content/css/skeleton.css")
                .Add("~/Content/css/new-general.css")
                .Add("~/Content/themes/base/jquery.ui.all.css")
                .Add("~/Content/themes/base/jquery.ui.theme.css")
                .Add("~/Content/themes/base/jquery.ui.core.css")
                .Add("~/Content/themes/base/jquery.ui.dialog.css")
                .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Render("/Content/combined/css/Registration_#.css")
                );
        }


        [BundleAppStartMethod]
        public static MvcHtmlString ScriptRegistrationBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Content/themes/Constellation/js/html5.js")
                .Add("/Content/themes/Constellation/js/old-browsers.js")
                .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                .Add("/Content/themes/Constellation/js/searchField.js")
                .Add("/Content/themes/Constellation/js/common.js")
                .Add("/Content/themes/Constellation/js/standard.js")
                .Add("/Content/themes/Constellation/js/jquery.tip.js")
                .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                .Add("/Content/themes/Constellation/js/jquery.modal.js")
                .Add("/Content/themes/Constellation/js/list.js")
                .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                .Add("/Scripts/underscore.js")
                .Add("/Scripts/jquery.dataTables-1.9.4.js")
                .Add("/Scripts/custom.js")
                .Add("/Scripts/BlockUI.js")
                .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                .Add("/Scripts/date.js")
                .Add("/Scripts/jquery.form.js")
                        .Render("/Content/combined/script/Registration_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleCEERegistrationBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
               .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/calendars.css")
                .Add("~/Content/themes/Constellation/css/960.gs.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/themes/Constellation/css/block-lists.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Add("~/Content/themes/Constellation/css/wizard.css")
                .Add("~/Content/themes/Constellation/css/gallery.css")
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                    .Render("/Content/combined/css/CEERegistration_#.css")
                );
        }
        #endregion

        #region SGOAuditTrail
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOAuditBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOAudit_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOAuditBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                     .Add("/Content/themes/AssignmentRegrader/js/css_browser_selector.js")
                     .Add("/Scripts/knockout-3.0.0.js")
                        .Render("/Content/combined/script/SGOAudit_#.js")
                );
        }

        #endregion

        #region SGOManage
        //StudentPopulation
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageStudentPopulationBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOStudentPopulate_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageStudentPopulationBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/SGOStudentPopulate_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOManageStudentPopulationBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                     .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Scripts/jquery.listSplitter.js")
                    .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Render("/Content/combined/script/SGOManagePopulate_#.js")
                );
        }

        //Index
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageIndexBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOManage_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageIndexBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/SGOManage_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOManageIndexBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Add("/Scripts/jquery.listSplitter.js")
                    .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                    .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/SGOManage_#.js")
                );
        }

        //FinalSignOff
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageFinalSignOffBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOManageSignOff_#.css")
                );
        }

        //FinalSignOff
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageFinalSignOffBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/SGOManageSignOff_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOManageFinalSignOffBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Render("/Content/combined/script/SGOManageSignOff_#.js")
                );
        }

        //EstablishStudent
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageEstablishStudentBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOManageEstablish_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageEstablishStudentBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/SGOManageEstablish_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOManageEstablishStudentBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/Constellation/js/common.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Render("/Content/combined/script/SGOManageEstablish_#.js")
                );
        }

        //AdminReview
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageAdminReviewBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOManageAdminReview_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOManageAdminReviewBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/SGOManageAdminReview_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOManageAdminReviewBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Scripts/knockout-3.0.0.js")
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Render("/Content/combined/script/SGOManageAdminReview_#.js")
                );
        }

        #endregion

        #region SGOProgressMonitor
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOProgressBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOProgress_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOProgressBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Scripts/knockout-3.0.0.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Content/themes/AssignmentRegrader/js/css_browser_selector.js")
                        .Render("/Content/combined/script/SGOProgress_#.js")
                );
        }

        #endregion

        #region SGOScoringPlanTarget
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOScoringPlanBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOScoringPlan_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSGOScoringPlanBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Scripts/knockout-3.0.0.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Content/themes/AssignmentRegrader/js/css_browser_selector.js")
                        .Render("/Content/combined/script/SGOScoringPlan_#.js")
                );
        }

        #endregion

        #region SGOSelectDataPoint
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOSelectDataPointBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOSelectDataPoint_#.css")
                );
        }
        #endregion

        #region Shared
        //Shared/_Layout
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSharedBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/form.css")
                    .Add("~/Content/themes/Constellation/css/standard.css")
                    .Add("~/Content/themes/Constellation/css/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/calendars.css")
                    .Add("~/Content/themes/Constellation/css/960.gs.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/themes/Constellation/css/block-lists.css")
                    .Add("~/Content/themes/Constellation/css/simple-lists.css")
                    .Add("~/Content/themes/Constellation/css/wizard.css")
                    .Add("~/Content/themes/Constellation/css/gallery.css")
                    .Add("~/Content/css/uploadifive.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/prettyPhoto.css")
                    .Add("~/Content/css/utilitySearch.css")
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/themes/base/jquery.ui.theme.css")
                    .Add("~/Content/themes/base/jquery.ui.core.css")
                    .Add("~/Content/themes/base/jquery.ui.dialog.css")
                    .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Add("~/Content/themes/base/jquery.ui.resizable.css")
                    .Add("~/Content/tipped/tipped.css")
                    .Add("~/Content/css/vue-components/vue-loading.css")
                    .Add("~/Content/css/vue-components.css")
                    .Render("/Content/combined/css/Common_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString NewStyleSharedBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/form.css")
                    .Add("~/Content/themes/Constellation/css/standard.css")
                    .Add("~/Content/themes/Constellation/css/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/calendars.css")
                    .Add("~/Content/themes/Constellation/css/960.gs.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/themes/Constellation/css/block-lists.css")
                    .Add("~/Content/themes/Constellation/css/simple-lists.css")
                    .Add("~/Content/themes/Constellation/css/wizard.css")
                    .Add("~/Content/themes/Constellation/css/gallery.css")
                    .Add("~/Content/css/uploadifive.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/prettyPhoto.css")
                    .Add("~/Content/css/utilitySearch.css")
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/themes/base/jquery.ui.theme.css")
                    .Add("~/Content/themes/base/jquery.ui.core.css")
                    .Add("~/Content/themes/base/jquery.ui.dialog.css")
                    .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Add("~/Content/themes/base/jquery.ui.resizable.css")
                    .Add("~/Content/tipped/tipped.css")
                    .Add("~/Content/css/vue-components/vue-loading.css")
                    .Add("~/Content/css/vue-components.css")
                    .Render("/Content/combined/css/Common_#.css")
                );
        }

        //Shared/_Layout_v2
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSharedBundleV2()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                    .Add("~/Content/css/portal-common-v2.css")
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/v2/form.css")
                    .Add("~/Content/themes/Constellation/css/v2/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/calendars.css")
                    .Add("~/Content/themes/Constellation/css/960.gs.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/themes/Constellation/css/block-lists.css")
                    .Add("~/Content/themes/Constellation/css/simple-lists.css")
                    .Add("~/Content/themes/Constellation/css/wizard.css")
                    .Add("~/Content/themes/Constellation/css/gallery.css")
                    .Add("~/Content/css/uploadifive.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/prettyPhoto.css")
                    .Add("~/Content/css/utilitySearch.css")
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/themes/base/jquery.ui.theme.css")
                    .Add("~/Content/themes/base/jquery.ui.core.css")
                    .Add("~/Content/themes/base/jquery.ui.dialog.css")
                    .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Add("~/Content/themes/base/jquery.ui.resizable.css")
                    .Add("~/Content/tipped/tipped.css")
                    .Add("~/Content/css/vue-components/vue-loading-v2.css")
                    .Add("~/Content/css/vue-components.css")
                    .Add("~/Content/libs/bootstrap/bootstrap.min.css")
                    .Add("~/Content/css/v2/dialog-custom.css")
                    .Add("~/Content/css/v2/dialog-ckeditor-custom.css")
                    .Render("/Content/combined/css/Common_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSharedBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Content/themes/Constellation/js/html5.js")
                .Add("/Content/themes/Constellation/js/old-browsers.js")
                .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                .Add("/Content/themes/Constellation/js/searchField.js")
                .Add("/Content/themes/Constellation/js/common.js")
                .Add("/Content/themes/Constellation/js/standard.js")
                .Add("/Content/themes/Constellation/js/jquery.tip.js")
                .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                .Add("/Content/themes/Constellation/js/jquery.modal.js")
                .Add("/Content/themes/Constellation/js/list.js")
                .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                .Add("/Scripts/underscore.js")
                .Add("/Scripts/jquery.dataTables-1.9.4.js")
                .Add("/Scripts/custom.js")
                .Add("/Scripts/BlockUI.js")
                .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                .Add("/Scripts/date.js")
                .Add("/Scripts/jquery.form.js")
                .Add("/Scripts/Lib/interact.min.js")
                .Add("/Scripts/SessionTimeOut.js")
                .Add("/Scripts/portal-common.js")
                .Render("/Content/combined/script/SharedLayout_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSharedBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Content/themes/Constellation/js/html5.js")
                .Add("/Content/themes/Constellation/js/old-browsers.js")
                .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                .Add("/Content/themes/Constellation/js/searchField.js")
                .Add("/Content/themes/Constellation/js/common.js")
                .Add("/Content/themes/Constellation/js/standard2.js")
                .Add("/Content/themes/Constellation/js/v2/jquery.tip.js")
                .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                .Add("/Content/themes/Constellation/js/jquery.modal.js")
                .Add("/Content/themes/Constellation/js/list.js")
                .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                .Add("/Scripts/underscore.js")
                .Add("/Scripts/jquery.dataTables-1.9.4-custom.js")
                .Add("/Scripts/customV2.js")
                .Add("/Scripts/BlockUI.js")
                .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                .Add("/Scripts/date.js")
                .Add("/FeLibs/jquery-form/jquery.form.min.js")
                .Add("/Scripts/Lib/interact.min.js")
                .Add("/Scripts/SessionTimeOut.js")
                .Add("/Scripts/DialogNewSkin/index.js")
                .Add("/Scripts/portal-common-v2.js")
                .Render("/Content/combined/script/SharedLayout_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSharedTopBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/jquery.ui.widget.js")
                .Add("/Scripts/jquery-ui-1.8.11.upgraded.js")
                .Add("/Scripts/jquery-ui-dialog-1.13.2.js")
                .Add("/FeLibs/jquery/jquery-ui-migrate.js")
                .Add("/Scripts/json2.js")
                .Add("/Scripts/uploadify/jquery.uploadifive.min.js")
                .Add("/Scripts/mediaelement.min.js")
                .Add("/Scripts/moment.min.js")
                .Add("/Scripts/Utils/Utils.js")
                .Add("/Scripts/Lib/vue.min.js")
                .Add("/Scripts/Lib/vue-infinite-scroll.js")
                .Add("/Scripts/Lib/vue-modal.js")
                .Add("/Scripts/Lib/vue-draggable-resizableV2.js")
                .Add("/Scripts/Lib/vue-loading-v2.js")
                .Render("/Content/combined/script/SharedTopLayout_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString jQuery()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/jquery-1.7.1.min.js")
                .Render("/Content/combined/script/jQuery_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString jQueryUpgrade()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/FeLibs/jquery/jquery.min.js")
                .Add("/FeLibs/jquery/jquery-migrate.min.js")
                .Add("/FeLibs/jquery/jquery-extension.js")
                .Render("/Content/combined/script/jQuery_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptSharedTopBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                .Add("/Scripts/jquery-ui-1.8.11.js")
                .Add("/FeLibs/jquery/jquery-ui-migrate.js")
                .Add("/Scripts/json2.js")
                .Add("/Scripts/uploadify/jquery.uploadifive.min.js")
                .Add("/Scripts/mediaelement.min.js")
                .Add("/Scripts/moment.min.js")
                .Add("/Scripts/Utils/Utils.js")
                .Add("/Scripts/Lib/vue.min.js")
                .Add("/Scripts/Lib/vue-infinite-scroll.js")
                .Add("/Scripts/Lib/vue-modal.js")
                .Add("/Scripts/Lib/vue-draggable-resizable.js")
                .Add("/Scripts/Lib/vue-loading.js")
                .Render("/Content/combined/script/SharedTopLayout_#.js")
                );
        }

        //Shared/_LayoutStudentLogin
        [BundleAppStartMethod]
        public static MvcHtmlString StyleLayoutStudentLoginBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/form.css")
                    .Add("~/Content/themes/Constellation/css/standard.css")
                    .Add("~/Content/themes/Constellation/css/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/calendars.css")
                    .Add("~/Content/themes/Constellation/css/960.gs.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/themes/Constellation/css/block-lists.css")
                    .Add("~/Content/themes/Constellation/css/simple-lists.css")
                    .Add("~/Content/themes/Constellation/css/wizard.css")
                    .Add("~/Content/themes/Constellation/css/gallery.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/prettyPhoto.css")
                    .Add("~/Content/css/utilitySearch.css")
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/themes/base/jquery.ui.theme.css")
                    .Add("~/Content/themes/base/jquery.ui.core.css")
                    .Add("~/Content/themes/base/jquery.ui.dialog.css")
                    .Add("~/Content/themes/base/jquery.ui.resizable.css")
                    .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Render("/Content/combined/css/SharedLayoutStudentLogin_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptLayoutStudentLoginBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/Constellation/js/html5.js")
                    .Add("/Content/themes/Constellation/js/old-browsers.js")
                    .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                    .Add("/Content/themes/Constellation/js/searchField.js")
                    .Add("/Content/themes/Constellation/js/common.js")
                    .Add("/Content/themes/Constellation/js/standard.js")
                    .Add("/Content/themes/Constellation/js/jquery.tip.js")
                    .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                    .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                    .Add("/Content/themes/Constellation/js/jquery.modal.js")
                    .Add("/Content/themes/Constellation/js/list.js")
                    .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                    .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                    .Add("/Scripts/underscore.js")
                    .Add("/Scripts/jquery.dataTables-1.9.4.js")
                    .Add("/Scripts/custom.js")
                    .Add("/Scripts/BlockUI.js")
                    .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                    .Add("/Scripts/date.js")
                    .Add("/Scripts/jquery.form.js")
                    .Render("/Content/combined/script/SharedLayoutStudentLogin_#.js")
                );
        }

        //Shared/_LogOnNetworkAdminPartial
        [BundleAppStartMethod]
        public static MvcHtmlString StyleLogOnNetworAdminPartialBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                    .Add("~/Content/themes/Constellation/css/reset.css")
                    .Add("~/Content/themes/Constellation/css/common.css")
                    .Add("~/Content/themes/Constellation/css/custom.css")
                    .Add("~/Content/themes/Constellation/css/form.css")
                    .Add("~/Content/themes/Constellation/css/standard.css")
                    .Add("~/Content/themes/Constellation/css/special-pages.css")
                    .Add("~/Content/themes/Constellation/css/calendars.css")
                    .Add("~/Content/themes/Constellation/css/960.gs.css")
                    .Add("~/Content/themes/Constellation/css/table.css")
                    .Add("~/Content/themes/Constellation/css/block-lists.css")
                    .Add("~/Content/themes/Constellation/css/simple-lists.css")
                    .Add("~/Content/themes/Constellation/css/wizard.css")
                    .Add("~/Content/themes/Constellation/css/gallery.css")
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/prettyPhoto.css")
                    .Add("~/Content/css/utilitySearch.css")
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/themes/base/jquery.ui.theme.css")
                    .Add("~/Content/themes/base/jquery.ui.core.css")
                    .Add("~/Content/themes/base/jquery.ui.dialog.css")
                    .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Render("/Content/combined/css/LogOnNetworAdminPartial_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptLogOnNetworAdminPartialBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/Constellation/js/html5.js")
                    .Add("/Content/themes/Constellation/js/old-browsers.js")
                    .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                    .Add("/Content/themes/Constellation/js/searchField.js")
                    .Add("/Content/themes/Constellation/js/common.js")
                    .Add("/Content/themes/Constellation/js/standard.js")
                    .Add("/Content/themes/Constellation/js/jquery.tip.js")
                    .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                    .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                    .Add("/Content/themes/Constellation/js/jquery.modal.js")
                    .Add("/Content/themes/Constellation/js/list.js")
                    .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                    .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                    .Add("/Scripts/underscore.js")
                    .Add("/Scripts/jquery.dataTables-1.9.4.js")
                    .Add("/Scripts/custom.js")
                    .Add("/Scripts/BlockUI.js")
                    .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                    .Add("/Scripts/date.js")
                    .Add("/Scripts/jquery.form.js")
                    .Render("/Content/combined/script/LogOnNetworAdminPartial_#.js")
                );
        }

        //Shared/_LogOnPartial
        [BundleAppStartMethod]
        public static MvcHtmlString StyleLogOnPartialBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                 .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/calendars.css")
                .Add("~/Content/themes/Constellation/css/960.gs.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/themes/Constellation/css/block-lists.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Add("~/Content/themes/Constellation/css/wizard.css")
                .Add("~/Content/themes/Constellation/css/gallery.css")
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                .Add("~/Content/themes/base/jquery.ui.all.css")
                .Add("~/Content/themes/base/jquery.ui.theme.css")
                .Add("~/Content/themes/base/jquery.ui.core.css")
                .Add("~/Content/themes/base/jquery.ui.dialog.css")
                .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                .Render("/Content/combined/css/LogOnPartial_#.css")
                );
        }


        [BundleAppStartMethod]
        public static MvcHtmlString ScriptLogOnPartialBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("/Content/themes/Constellation/js/html5.js")
                    .Add("/Content/themes/Constellation/js/old-browsers.js")
                    .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                    .Add("/Content/themes/Constellation/js/searchField.js")
                    .Add("/Content/themes/Constellation/js/common.js")
                    .Add("/Content/themes/Constellation/js/standard.js")
                    .Add("/Content/themes/Constellation/js/jquery.tip.js")
                    .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                    .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                    .Add("/Content/themes/Constellation/js/jquery.modal.js")
                    .Add("/Content/themes/Constellation/js/list.js")
                    .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                    .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                    .Add("/Scripts/underscore.js")
                    .Add("/Scripts/jquery.dataTables-1.9.4.js")
                    .Add("/Scripts/custom.js")
                    .Add("/Scripts/BlockUI.js")
                    .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                    .Add("/Scripts/date.js")
                    .Add("/Scripts/jquery.form.js")
                    .Render("/Content/combined/script/LogOnPartial_#.js")
                );
        }
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptLogOnPartialBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("/Content/themes/Constellation/js/html5.js")
                    .Add("/Content/themes/Constellation/js/old-browsers.js")
                    .Add("/Content/themes/Constellation/js/jquery.accessibleList.js")
                    .Add("/Content/themes/Constellation/js/searchField.js")
                    .Add("/Content/themes/Constellation/js/common.js")
                    .Add("/Content/themes/Constellation/js/standard.js")
                    .Add("/Content/themes/Constellation/js/jquery.tip.js")
                    .Add("/Content/themes/Constellation/js/jquery.hashchange.js")
                    .Add("/Content/themes/Constellation/js/jquery.contextMenu.js")
                    .Add("/Content/themes/Constellation/js/jquery.modal.js")
                    .Add("/Content/themes/Constellation/js/list.js")
                    .Add("/Content/themes/Constellation/js/jquery.datepick/jquery.datepick.min.js")
                    .Add("/Scripts/jquery.maskedinput-1.2.2.js")
                    .Add("/Scripts/underscore.js")
                    .Add("/Scripts/jquery.dataTables-1.9.4.js")
                    .Add("/Scripts/customV2.js")
                    .Add("/Scripts/BlockUI.js")
                    .Add("/Scripts/prettyPhoto/jquery.prettyPhoto.js")
                    .Add("/Scripts/date.js")
                    .Add("/Scripts/jquery.form.js")
                    .Render("/Content/combined/script/LogOnPartial_#.js")
                );
        }

        //Shared/Error
        [BundleAppStartMethod]
        public static MvcHtmlString StyleErrorBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Render("/Content/combined/css/Error_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptErrorBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/Constellation/js/html5.js")
                    .Add("/FeLibs/jquery/jquery.min.js")
                    .Add("/FeLibs/jquery/jquery-migrate.min.js")
                    .Add("/FeLibs/jquery/jquery-extension.js")
                    .Add("/Content/themes/Constellation/js/common.js")
                    .Add("/Content/themes/Constellation/js/standard.js")
                    .Add("/Content/themes/Constellation/js/jquery.tip.js")
                    .Add("/Content/themes/Constellation/js/list.js")
                    .Render("/Content/combined/script/Error_#.js")
                );
        }

        //Shared/NotFound
        [BundleAppStartMethod]
        public static MvcHtmlString StyleNotFoundBundle()
        {
            return MvcHtmlString.Create(
                   Bundle.Css()
                .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Render("/Content/combined/css/NotFound_#.css")
                );
        }

        #endregion

        #region ShowQTIItem
        [BundleAppStartMethod]
        public static MvcHtmlString StyleShowQTIItemBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                 .Add("~/Content/themes/Constellation/css/reset.css")
                .Add("~/Content/themes/Constellation/css/common.css")
                .Add("~/Content/themes/Constellation/css/custom.css")
                .Add("~/Content/themes/Constellation/css/form.css")
                .Add("~/Content/themes/Constellation/css/standard.css")
                .Add("~/Content/themes/Constellation/css/special-pages.css")
                .Add("~/Content/themes/Constellation/css/calendars.css")
                .Add("~/Content/themes/Constellation/css/960.gs.css")
                .Add("~/Content/themes/Constellation/css/table.css")
                .Add("~/Content/themes/Constellation/css/block-lists.css")
                .Add("~/Content/themes/Constellation/css/simple-lists.css")
                .Add("~/Content/themes/Constellation/css/wizard.css")
                .Add("~/Content/themes/Constellation/css/gallery.css")
                .Add("~/Content/css/custom.css")
                .Add("~/Content/css/prettyPhoto.css")
                .Add("~/Content/css/utilitySearch.css")
                .Add("~/Content/themes/base/jquery.ui.all.css")
                .Add("~/Content/themes/base/jquery.ui.theme.css")
                .Add("~/Content/themes/base/jquery.ui.core.css")
                .Add("~/Content/themes/base/jquery.ui.dialog.css")
                .Add("~/Content/themes/base/jquery.ui.resizable.css")
                .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                .Add("~/Content/themes/LinkitStyleSheet.css")
                .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                .Add("~/Content/themes/AssignmentRegrader/css/tooltipster.css")
                .Add("~/Content/themes/ShowQtiItem/ShowQtiItem.css")
                    .Render("/Content/combined/css/ShowQTIItem_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptShowQTIItemBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                         .Add("/Content/themes/TestMaker/jquery.tooltipster.min.js")
                        .Add("/Content/themes/TestMaker/mediaelement.min.js")
                        .Add("/Scripts/ShowQtiItem/ShowQtiItem.js")
                        .Add("/Scripts/ShowQtiItem/ShowGuidanceRationale.js")
                        .Add("/Scripts/ShowQtiItem/ShowPassage.js")
                        .Render("/Content/combined/script/ShowQTIItem_#.js")
                );
        }

        #endregion

        #region StudentPassThrough
        [BundleAppStartMethod]
        public static MvcHtmlString StyleStudentPassThroughBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                        .Add("~/Content/themes/Constellation/css/reset.css")
                        .Add("~/Content/themes/Constellation/css/common.css")
                        .Add("~/Content/themes/Constellation/css/custom.css")
                        .Add("~/Content/themes/Constellation/css/form.css")
                        .Add("~/Content/themes/Constellation/css/standard.css")
                        .Add("~/Content/themes/Constellation/css/special-pages.css")
                        .Add("~/Content/themes/Constellation/css/calendars.css")
                        .Add("~/Content/themes/Constellation/css/960.gs.css")
                        .Add("~/Content/themes/Constellation/css/table.css")
                        .Add("~/Content/themes/Constellation/css/block-lists.css")
                        .Add("~/Content/themes/Constellation/css/simple-lists.css")
                        .Add("~/Content/themes/Constellation/css/wizard.css")
                        .Add("~/Content/themes/Constellation/css/gallery.css")
                        .Add("~/Content/css/custom.css")
                        .Add("~/Content/css/prettyPhoto.css")
                        .Add("~/Content/css/utilitySearch.css")
                    .Render("/Content/combined/css/StudentPassThrough_#.css")
                );
        }

        #endregion

        #region StudentPreference
        [BundleAppStartMethod]
        public static MvcHtmlString StyleStudentPreferenceBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/StudentPreferences/styleSheet.css")
                    .Add("~/Content/themes/linkitStyleSheet.css")
                    .Render("/Content/combined/css/StudentPreference_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptStudentPreferenceBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Scripts/StudentPreference/Util.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Render("/Content/combined/script/StudentPreference_#.js")
                );
        }
        #endregion

        #region TDLSManage
        //UpcommingSchoolSubmit
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSUpcommingSchoolBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/TDLSUpcommingSchool_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSUpcommingSchoolBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/v2/sgo-home.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/TDLSUpcommingSchool_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSUpcommingSchoolBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TDLSUpcommingSchool_#.js")
                );
        }

        //Index
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSIndexBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/tlds.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/TDLS_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSIndexBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Add("/Scripts/moment.min.js")
                        .Render("/Content/combined/script/TDLS_#.js")
                );
        }

        //EnhancedTransitions
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSEnhancedTransitionsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/TDLSEnhancedTransitions_#.css")
                );
        }

        //EnhancedTransitionsV2
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSEnhancedTransitionsBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/TDLSEnhancedTransitions_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSEnhancedTransitionsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TDLSEnhancedTransitions_#.js")
                );
        }


        //Edit
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSEditBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/TDLSEdit_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSEditBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/TDLSEdit_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSEditBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TDLSEdit_#.js")
                );
        }

        //DevelopmentOutCome
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSDevelopmentOutComeBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/TDLSDevelopmentOutCome_#.css")
                );
        }

        //DevelopmentOutCome V2
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSDevelopmentOutComeBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/TDLSDevelopmentOutCome_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSDevelopmentOutComeBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Scripts/Lib/vue.min.js")
                        .Add("/Scripts/Lib/vue-modal.js")
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TDLSDevelopmentOutCome _#.js")
                );
        }


        //ContextSpecificInfor
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSContextSpecificInforBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/TDLSContextSpecificInfor_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSContextSpecificInforBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/v2/sgo-home.css")
                    .Render("/Content/combined/css/TDLSContextSpecificInfor_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSContextSpecificInforBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TDLSContextSpecificInfor _#.js")
                );
        }

        //Configuration
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSConfigurationBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Render("/Content/combined/css/TDLSConfiguration_#.css")
                );
        }

        //ChildFamily
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSChildFamilyBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/TDLSChildFamily_#.css")
                );
        }

        //ChildFamilyV2
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTDLSChildFamilyBundleV2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/v2/sgo-home.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/TDLSChildFamily_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTDLSChildFamilyBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                        .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TDLSChildFamily_#.js")
                );
        }
        #endregion

        #region TeacherReview
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTeacherReviewBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/AssignmentRegrader/css/popup.css")
                    .Add("~/Content/css/linkitStyleSheet.css")
                    .Add("~/Content/themes/AssignmentRegrader/css/tooltipster.css")
                    .Add("~/Content/css/select2.css")
                    .Add("~/Content/css/assignment-review.css")
                    .Add("~/Content/css/tippy-theme-light.css")
                    .Render("/Content/combined/css/TeacherReview_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTeacherReviewBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript().ForceDebug()
                    .Add("/Scripts/mxgraph/mxClient.js")
                    .Add("/Scripts/select2-4.0.0.full.min.js")
                    .Add("/Scripts/moment.min.js")
                    .Add("/Scripts/date.js")
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Scripts/imagesloaded.pkgd.js")
                    .Add("/Scripts/Linkit/LinkitUtils.js")
                    .Add("/Scripts/Linkit/LinkitQuestion.js")
                    .Add("/Scripts/RecordRTC/RecordRTC.js")
                    .Add("/Scripts/RecordRTC/RecordRTCBase.js")
                    .Add("/Scripts/RecordRTC/plugin.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/jquery.selectbox.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Knockout/knockout-select2.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Knockout/knockout-dialog.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Knockout/knockout-showblock.js")
                    .Add("/Scripts/TeacherReviewerFullPage/AudioPlayer.js")
                    .Add("/Scripts/TeacherReviewerFullPage/ReviewerUtil.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Model/QuestionFilter.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/ComplexItem.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/SimpleChoice.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/MultipleChoice.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/InlineChoice.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/TextEntry.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/OpenEnded.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DrawingBasic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DragDropStandard.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/QuestionRender.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/TextHotspot.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/ImgHotspot.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/TableHotspot.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/Numberline.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DragDropSequence.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DragDropNumerical.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/MultipleChoiceVariable.js")
                    .Add("/Scripts/TeacherReviewerFullPage/AssignmentReviewer.js")
                    .Add("/Scripts/TeacherReviewerFullPage/ReviewerWidget.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Widget/GradingShortcutsWidget.js")
                    .Add("/Scripts/TeacherReviewerFullPage/ReviewerValidationWidget.js")
                    .Add("/Scripts/TeacherReviewerFullPage/StudentFilterWidget.js")
                    .Add("/Scripts/TeacherReviewerFullPage/ReviewerModel.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/Glossary.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/Rationale.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/SimpleChoiceAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/MultipleChoiceAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/MultipleChoiceVariableAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/InlineChoiceAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/TextEntryAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DragDropStandardAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/TextHotspotAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/ImgHotspotAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/TableHotspotAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/NumberlineAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DragDropSequenceAlgorithmic.js")
                    .Add("/Scripts/TeacherReviewerFullPage/Questions/DragDropNumericalAlgorithmic.js")
                    .Add("/Scripts/qtiItemLoadMedia.js")
                    .Render("/Content/combined/script/TeacherReview_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTeacherReviewBundleV2()
        {
            var scripts = new string[]
            {
                "/Scripts/mxgraph/mxClient.js",
                "/Scripts/select2.full.min.js",
                "/Scripts/moment.min.js",
                "/Scripts/date.js",
                "/Scripts/knockout-3.0.0.js",
                "/Scripts/imagesloaded.pkgd.js",
                "/Scripts/Linkit/LinkitUtils.js",
                "/Scripts/Linkit/LinkitQuestion.js",
                "/Scripts/RecordRTC/RecordRTC.js",
                "/Scripts/RecordRTC/RecordRTCBase.js",
                "/Scripts/RecordRTC/plugin.js",
                "/Content/themes/TestMaker/mediaelement-and-player.min.js",
                "/Content/themes/TestMaker/jquery.tooltipster.min.js",
                "/Scripts/TeacherReviewerFullPage/Questions/jquery.selectbox.js",
                "/Scripts/TeacherReviewerFullPage/Knockout/knockout-select2.js",
                "/Scripts/TeacherReviewerFullPage/Knockout/knockout-dialog.js",
                "/Scripts/TeacherReviewerFullPage/Knockout/knockout-showblock.js",
                "/Scripts/TeacherReviewerFullPage/AudioPlayer.js",
                "/Scripts/TeacherReviewerFullPage/v2/ReviewerUtil.js",
                "/Scripts/TeacherReviewerFullPage/Model/QuestionFilter.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/ComplexItem.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/SimpleChoice.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/MultipleChoice.js",
                "/Scripts/TeacherReviewerFullPage/Questions/OpenEnded.js",
                "/Scripts/TeacherReviewerFullPage/Questions/DrawingBasic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/DragDropStandard.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/DragDropStandard.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/QuestionRender.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/TextHotspot.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/ImgHotspot.js",
                "/Scripts/TeacherReviewerFullPage/Questions/TableHotspot.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/TableHotspot.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/Numberline.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/DragDropSequence.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/DragDropNumerical.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/MultipleChoiceVariable.js",
                "/Scripts/TeacherReviewerFullPage/AssignmentReviewer.js",
                "/Scripts/TeacherReviewerFullPage/v2/ReviewerWidget.js",
                "/Scripts/TeacherReviewerFullPage/Widget/GradingShortcutsWidget.js",
                "/Scripts/TeacherReviewerFullPage/ReviewerValidationWidget.js",
                "/Scripts/TeacherReviewerFullPage/StudentFilterWidget.js",
                "/Scripts/TeacherReviewerFullPage/v2/ReviewerModel.js",
                "/Scripts/TeacherReviewerFullPage/Questions/Glossary.js",
                "/Scripts/TeacherReviewerFullPage/Questions/Rationale.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/SimpleChoiceAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/MultipleChoiceAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/MultipleChoiceVariableAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/InlineChoiceAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/InlineChoiceAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/TextEntryAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/TextEntryAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/DragDropStandardAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/DragDropStandardAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/TextHotspotAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/TextHotspotAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/ImgHotspotAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/ImgHotspotAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/TableHotspotAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/TableHotspotAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/NumberlineAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/NumberlineAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/DragDropSequenceAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/DragDropNumericalAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/DragDropNumericalAlgorithmic.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/InlineChoice.js",
                "/Scripts/TeacherReviewerFullPage/Questions/v2/TextEntry.js",
                "/Scripts/qtiItemLoadMedia.js"
            };
            var bundler = Bundle.JavaScript().ForceDebug();
            foreach (var script in scripts)
            {
                bundler.Add(Version(script));
            }
            return @MvcHtmlString.Create(bundler.Render("/Content/combined/script/TeacherReview_#.js"));
        }
        #endregion

        #region Test


        #endregion

        #region TestAssignmentRegrader
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTestAssignmentRegraderBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/AssignmentRegrader/css/bootstrap.css")
                    .Add("~/Content/themes/AssignmentRegrader/css/popup.css")
                    .Add("~/Content/css/linkitStyleSheet.css")
                    .Add("~/Content/themes/AssignmentRegrader/css/tooltipster.css")
                    .Render("/Content/combined/css/TestAssignmentRegrader_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTestAssignmentRegraderBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/AssignmentRegrader/js/css_browser_selector.js")
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Scripts/imagesloaded.pkgd.js")
                    .Add("/Scripts/Linkit/LinkitUtils.js")
                    .Add("/Scripts/Linkit/LinkitQuestion.js")
                    .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                    .Add("/Scripts/TeacherReviewer/Reviewer.js")
                    .Add("/Scripts/TeacherReviewer/Questions/OpenEnded.js")
                    .Add("/Scripts/TeacherReviewer/Questions/SimpleChoice.js")
                    .Add("/Scripts/TeacherReviewer/Questions/MultipleChoice.js")
                    .Add("/Scripts/TeacherReviewer/Questions/MultipleChoiceVariable.js")
                    .Add("/Scripts/TeacherReviewer/Questions/InlineChoice.js")
                    .Add("/Scripts/TeacherReviewer/Questions/TextEntry.js")
                    .Add("/Scripts/TeacherReviewer/Questions/ComplexItem.js")
                    .Add("/Scripts/TeacherReviewer/Questions/DragDropStandard.js")
                    .Add("/Scripts/TeacherReviewer/Questions/DragDropSequence.js")
                    .Add("/Scripts/TeacherReviewer/Questions/TextHotspot.js")
                    .Add("/Scripts/TeacherReviewer/Questions/ImgHotspot.js")
                    .Add("/Scripts/TeacherReviewer/Questions/Numberline.js")
                    .Add("/Scripts/TeacherReviewer/Questions/TableHotspot.js")
                    .Add("/Scripts/TeacherReviewer/Questions/DragDropNumerical.js")
                    .Add("/Scripts/qtiItemLoadMedia.js")
                    .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                    .Add("/Scripts/TeacherReviewer/Questions/jquery.selectbox.js")
                    .Add("/Content/themes/TestMaker/jquery.tooltipster.min.js")
                    .Add("/Scripts/TeacherReviewer/Questions/Glossary.js")
                    .Add("/Scripts/TeacherReviewer/Questions/Rationale.js")
                        .Render("/Content/combined/script/TestAssignmentRegrader_#.js")
                );
        }

        #endregion

        #region TestMaker
        //Index
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTestMakerBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                    .Add("~/Content/themes/TestMaker/contents.css")
                    .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Add("~/Scripts/Texttospeech/texttospeech.css")
                    .Render("/Content/combined/css/TestMaker_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTestMakerBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                    .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                        .Render("/Content/combined/script/TestMaker_#.js")
                );
        }

        //Edit
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTestMakerEditBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                    .Add("~/Content/themes/TestMaker/contents.css")
                    .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Add("~/Content/themes/base/jquery.ui.dialog.css")
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/css/dialog-draggable.css")
                    .Add("~/Scripts/Texttospeech/texttospeech.css")
                    .Render("/Content/combined/css/TestMakerEdit_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTestMakerEditBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                    .Add("/FeLibs/raphael/raphael.min.js")
                    .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                    .Add("~/Scripts/qtiItemLoadMedia.js")
                        .Render("/Content/combined/script/TestMakerEdit_#.js")
                );
        }

        #endregion

        #region TestResultsExport
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTestResultsExportBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                     .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                     .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TestResultsExport_#.js")
                );
        }
        #endregion

        #region TLDSReport
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTLDSReportBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/sgohome.css")
                    .Add("~/Content/css/tlds.css")
                    .Render("/Content/combined/css/TLDSReport_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTLDSReportBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                       .Add("/Content/themes/TestMaker/ckeditor_utils.js")
                        .Add("/Scripts/jquery.listSplitter.js")
                        .Add("/FeLibs/jquery-coolfieldset/js/jquery.coolfieldset.js")
                        .Add("/Scripts/custom.js")
                        .Render("/Content/combined/script/TLDSReport_#.js")
                );
        }

        #endregion

        #region Upload3pItemBank
        [BundleAppStartMethod]
        public static MvcHtmlString StyleUpload3pItemBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                     .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                     .Add("~/Content/themes/TestMaker/contents.css")
                     .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Render("/Content/combined/css/Upload3pItem_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString StyleUpload3pItemBundle2()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                   .Add("~/Content/themes/AssessmentItem/css/popup.css")
                   .Add("~/Content/themes/LinkitStyleSheet.css")
                   .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                    .Render("/Content/combined/css/Upload3pItem_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptUpload3pItemBankBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/Scripts/knockout-3.0.0.js")
                    .Add("/Content/themes/TestMaker/mediaelement-and-player.min.js")
                        .Render("/Content/combined/script/Upload3pItem_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptUpload3pItemBankBundle2()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                      .Add("/Scripts/imagesloaded.pkgd.js")
                      .Add("/Scripts/PrintTest/PrintTest.js")
                        .Render("/Content/combined/script/Upload3pItem_#.js")
                );
        }

        #endregion

        #region VirtualTest
        //Index
        [BundleAppStartMethod]
        public static MvcHtmlString StyleVirtualTestBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                    .Add("~/Content/themes/AssessmentItem/css/popup.css")
                    .Add("~/Content/themes/TestMaker/contents.css")
                    .Add("~/Content/themes/LinkitStyleSheet.css")
                    .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                    .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Add("~/Scripts/Texttospeech/texttospeech.css")
                    .Render("/Content/combined/css/VirtualTest_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptVirtualTestBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                    .Add("/Scripts/qtiItemLoadMedia.js")
                    .Render("/Content/combined/script/VirtualTest_#.js")
                );
        }

        //ImportItemsFromLibrary
        [BundleAppStartMethod]
        public static MvcHtmlString StyleVirtualTestImportItemsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/import-item.css")
                     .Add("~/Content/themes/TestMaker/ckeditor_mk.css")
                    .Add("~/Content/themes/AssessmentItem/css/popup.css")
                    .Add("~/Content/themes/TestMaker/contents.css")
                    .Add("~/Content/themes/LinkitStyleSheet.css")
                    .Add("~/Content/themes/Print/ItemSets/ItemSet.css")
                    .Add("~/Content/themes/TestDesign/TestDesign.css")
                    .Add("~/Content/themes/base/jquery.ui.autocomplete.css")
                    .Add("~/Content/themes/TestDesign/TreeView.css")
                    .Add("~/Scripts/Texttospeech/texttospeech.css")
                    .Render("/Content/combined/css/VirtualTestImportItems_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptVirtualTestImportItemsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                    .Add("/Scripts/qtiItemLoadMedia.js")
                    .Add("/Scripts/imagesloaded.pkgd.js")
                    .Add("/Scripts/PrintTest/PrintTest.js")
                     .Add("/Scripts/knockout-3.0.0.js")
                     .Add("/Scripts/jquery.coolfieldset.js")
                     .Add("/Scripts/ImportItem/script.js")
                    .Render("/Content/combined/script/VirtualTestImportItems_#.js")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptItemLibraryImportItemsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("~/Scripts/Texttospeech/responsivevoice.js")
                    .Add("~/Scripts/Texttospeech/text-to-speech-plugins.js")
                    .Add("/Scripts/qtiItemLoadMedia.js")
                    .Add("/Scripts/imagesloaded.pkgd.js")
                    .Add("/Scripts/PrintTest/PrintTest.js")
                     .Add("/Scripts/knockout-3.0.0.js")
                     .Add("/Scripts/jquery.coolfieldset.js")
                     .Add("/Scripts/ItemLibrary/script.js")
                    .Render("/Content/combined/script/VirtualTestImportItems_#.js")
                );
        }

        #endregion

        #region Single file
        //custom
        [BundleAppStartMethod]
        public static MvcHtmlString StyleCustomBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/css/custom.css")
                    .Add("~/Content/css/vue-components/vue-modal.css")
                    .Render("/Content/combined/css/custom_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptCustomBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("~/Scripts/custom.js")
                        .Render("/Content/combined/script/Custom_#.js")
                );
        }

        //sgohome
        [BundleAppStartMethod]
        public static MvcHtmlString StyleSGOHomeBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("/Content/css/sgohome.css")
                    .Render("/Content/combined/css/SGOHome_#.css")
                );
        }

        //knocout3.0
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptKnockout30Bundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("/Scripts/knockout-3.0.0.js")
                        .Render("/Content/combined/script/knockout_#.js")
                );
        }

        //CKEditor_Utils
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptCKEditorUtilsBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("~/Content/themes/TestMaker/ckeditor_utils.js")
                        .Render("/Content/combined/script/combined_#.js")
                );
        }

        //LKSearchWidget
        [BundleAppStartMethod]
        public static MvcHtmlString ScriptLKSearchWidgetBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("~/Scripts/LKSearchWidget.min.js")
                        .Render("/Content/combined/script/combined_#.js")
                );
        }

        public static MvcHtmlString Include(string[] urls)
        {
            var jsBundle = Bundle.JavaScript().ForceDebug();
            var cssBundle = Bundle.Css().ForceDebug();
            foreach(var url in urls)
            {
                if (url.EndsWith(".js"))
                {
                    jsBundle = jsBundle.Add(Version(url));
                } else if (url.EndsWith(".css"))
                {
                    cssBundle = cssBundle.Add(Version(url));
                }
            }
            return @MvcHtmlString.Create(
                    cssBundle.Render(string.Empty) + "\n" + jsBundle.Render(string.Empty)
                );
        }

        public static MvcHtmlString Include(string url)
        {
            return Include(new string[] { url });
        }

        public static string Version(string rootRelativePath)
        {
            if (String.IsNullOrEmpty(rootRelativePath))
            {
                throw new ArgumentNullException("Null exception", "rootRelativePath");
            }

            if (HttpRuntime.Cache[rootRelativePath] == null)
            {
                string absolutePath = HostingEnvironment.MapPath(rootRelativePath);
                DateTime lastChangedDateTime;
                string versionedUrl;

                if (File.Exists(absolutePath))
                {
                    lastChangedDateTime = File.GetLastWriteTime(absolutePath);
                    string newRelativePath = rootRelativePath;
                    if (newRelativePath.StartsWith("~"))
                    {
                        newRelativePath = newRelativePath.Substring(1);
                    }
                    versionedUrl = newRelativePath + "?v=" + lastChangedDateTime.Ticks;
                } else if (Directory.Exists(absolutePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(absolutePath);
                    FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
                    lastChangedDateTime = files.Max(e => e.LastWriteTime);
                    versionedUrl = "?v=" + lastChangedDateTime.Ticks;
                } else
                {
                    throw new DirectoryNotFoundException("The specified path does not exist or is invalid.");
                }

                HttpRuntime.Cache.Insert(rootRelativePath, versionedUrl, new CacheDependency(absolutePath));
            }

            return HttpRuntime.Cache[rootRelativePath] as string;
        }

        #endregion

        #region TLDS Digital Section 2 & 3
        [BundleAppStartMethod]
        public static MvcHtmlString StyleTLDSDigitalSection23Bundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/Content/themes/base/jquery.ui.all.css")
                    .Add("~/Content/themes/base/jquery.ui.theme.css")
                    .Add("~/Content/themes/base/jquery.ui.core.css")
                    .Add("~/Content/themes/base/jquery.ui.datepicker.css")
                    .Add("~/Content/themes/TLDSDigitalSections23/css/bootstrap.css")
                    .Add("~/Content/themes/Print/TLDSPrint/TLDSPrint.css")
                    .Add("~/Content/themes/TLDSDigitalSections23/css/style.css")
                    .Render("/Content/combined/css/TLDSDigitalSection23_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString ScriptTLDSDigitalSection23Bundle()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                    .Add("/FeLibs/jquery/jquery.min.js")
                    .Add("/Content/themes/TLDSDigitalSections23/js/bootstrap.js")
                    .Add("~/Scripts/jquery.validate.min.js")
                    .Add("~/Scripts/jquery.validate.unobtrusive.min.js")
                    .Render("/Content/combined/script/TLDSDigitalSection23_#.js")
                );
        }
        #endregion
        #region Demo
        [BundleAppStartMethod]
        public static MvcHtmlString DemoCss()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                    .Add("~/app/css/demo/style.css")
                    .Render("/Content/combined/css/demo_#.css")
                );
        }

        [BundleAppStartMethod]
        public static MvcHtmlString DemoJs()
        {
            return @MvcHtmlString.Create(
                Bundle.JavaScript()
                   .Add("~/app/js/demo/index.js")
                        .Render("/Content/combined/script/demo_#.js")
                );
        }
        #endregion

        #region Home
        [BundleAppStartMethod]
        public static MvcHtmlString StyleHomeIndexBundle()
        {
            return @MvcHtmlString.Create(
                Bundle.Css()
                .Add("~/Content/css/nivo-slider.css")
                .Add("~/Content/css/v2/homePage.css")
                    .Render("/Content/combined/css/Home_#.css")
                );
        }
        #endregion

    }
}
