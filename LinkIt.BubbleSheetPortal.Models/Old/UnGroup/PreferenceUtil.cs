using LinkIt.BubbleSheetPortal.Common.Enum;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PreferenceUtil
    {
        public static List<int> GetOverrideItems(int roleID, bool overrideAutoGraded, bool isAlgorithmic = false)
        {
            {
                var overrideItems = new List<int>();

                if (overrideAutoGraded || isAlgorithmic || RoleUtil.IsPublisher(roleID) || RoleUtil.IsNetworkAdmin(roleID) || RoleUtil.IsDistrictAdmin(roleID))
                {
                    overrideItems.Add((int)QtiSchemaEnum.MultipleChoice);
                    overrideItems.Add((int)QtiSchemaEnum.MultiSelect);
                    overrideItems.Add((int)QtiSchemaEnum.InlineChoice);
                    overrideItems.Add((int)QtiSchemaEnum.TextEntry);
                    overrideItems.Add((int)QtiSchemaEnum.ExtendedText);
                    overrideItems.Add((int)QtiSchemaEnum.DragAndDrop);
                    overrideItems.Add((int)QtiSchemaEnum.Complex);
                    overrideItems.Add((int)QtiSchemaEnum.TextHotSpot);
                    overrideItems.Add((int)QtiSchemaEnum.ImageHotSpot);
                    overrideItems.Add((int)QtiSchemaEnum.TableHotSpot);
                    overrideItems.Add((int)QtiSchemaEnum.NumberLineHotSpot);
                    overrideItems.Add((int)QtiSchemaEnum.DragAndDropNumerical);
                    overrideItems.Add((int)QtiSchemaEnum.DragAndDropSequence);
                    overrideItems.Add((int)QtiSchemaEnum.ChoiceMultipleVariable);
                }
                else if (RoleUtil.IsSchoolAdmin(roleID) || RoleUtil.IsTeacher(roleID))
                {
                    overrideItems.Add((int)QtiSchemaEnum.TextEntry);
                    overrideItems.Add((int)QtiSchemaEnum.DragAndDrop);
                    overrideItems.Add((int)QtiSchemaEnum.DragAndDropNumerical);
                    overrideItems.Add((int)QtiSchemaEnum.DragAndDropSequence);
                    overrideItems.Add((int)QtiSchemaEnum.Complex);
                    overrideItems.Add((int)QtiSchemaEnum.ExtendedText);
                }

                return overrideItems;
            }
        }
    }
}
