using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryMappingInformationRepository : IRepository<MappingInformation>
    {
        private List<MappingInformation> table;
        private static int nextUniqueID = 1;

        public InMemoryMappingInformationRepository()
        {
            table = AddMappingInformation();
        }

        private List<MappingInformation> AddMappingInformation()
        {
            return new List<MappingInformation>
                       {
                           new MappingInformation
                           { 
                               MapID=1, 
                               LoaderType = MappingLoaderType.DataLoader, 
                               CreatedDate = DateTime.Now, 
                               LastestModifiedDate = DateTime.Now,
                               Name="test mapping 1",
                               ProgressStatus = MappingProgressStatus.InProgress,
                               SourceFileContent = "Student ID	First_Name	Last_Name	Grade 08_09	School 08_09	Homeroom Teacher 08_09	Ethnicity	Gender	LEP	Section 504	Special Education	TerraNova 08_09 Language NP	TerraNova 08_09 Math NP	TerraNova 08_09 Reading NP	TerraNova 08_09 Total Score NP	TerraNova 08_09 Lang Edit Skill	TerraNova 08_09 Lang Sent Struct	TerraNova 08_09 Lang Wrt Strat	TerraNova 08_09 Language NCE	TerraNova 08_09 Math Alg Pattern Func	TerraNova 08_09 Math Communi	TerraNova 08_09 Math Comp Est	TerraNova 08_09 Math Data Stat Prob	TerraNova 08_09 Math Geom Spat Sns	TerraNova 08_09 Math Measurement	TerraNova 08_09 Math NCE	TerraNova 08_09 Math Numb Num Rel	TerraNova 08_09 Math Oper Concept	TerraNova 08_09 Math Prob Solv	TerraNova 08_09 Read Anal Text	TerraNova 08_09 Read Basic Understnd	TerraNova 08_09 Read Eval Ext Mng	TerraNova 08_09 Read Intro Print	TerraNova 08_09 Read Rdg Wrt Strat	TerraNova 08_09 Reading NCE	TerraNova 08_09 Total Score NCE		\r\n"
                                                    +"23364	Tahniyat	Abidi	5	110	Mrs. Cullen	A	F				78	78	67	77	85	87	68	66	77	61	79	71	63	71	66	69	92	66	59	73	77		63	59	66		Delete TRNV Group Data\r\n"
                                                    +"23086	Nicholas	Abramo	5	110	Mrs. Closius	W	M			17	45	61	48	50	68	64	52	48	64	50	68	63	54	58	56	56	86	50	47	64	70		54	49	50		NP = Percentile\r\n"
                                                    +"23676	Devyn	Accardi	5	75	Ms. Blaker	W	F				98	99	96	99	98	99	84	93	97	87	97	93	87	95	99	92	99	93	80	93	92		82	86	99		What is NCE?",
                               UserID= 10,
                               XmlTransform = string.Empty
                           },
                           new MappingInformation
                           { 
                               MapID=2, 
                               LoaderType = MappingLoaderType.DataLoader, 
                               CreatedDate = DateTime.Now, 
                               LastestModifiedDate = DateTime.Now,
                               Name="test mapping 2",
                               ProgressStatus = MappingProgressStatus.InProgress,
                               SourceFileContent = "Student ID	First_Name	Last_Name	Grade 08_09	School 08_09	Homeroom Teacher 08_09	Ethnicity	Gender	LEP	Section 504	Special Education	TerraNova 08_09 Language NP	TerraNova 08_09 Math NP	TerraNova 08_09 Reading NP	TerraNova 08_09 Total Score NP	TerraNova 08_09 Lang Edit Skill	TerraNova 08_09 Lang Sent Struct	TerraNova 08_09 Lang Wrt Strat	TerraNova 08_09 Language NCE	TerraNova 08_09 Math Alg Pattern Func	TerraNova 08_09 Math Communi	TerraNova 08_09 Math Comp Est	TerraNova 08_09 Math Data Stat Prob	TerraNova 08_09 Math Geom Spat Sns	TerraNova 08_09 Math Measurement	TerraNova 08_09 Math NCE	TerraNova 08_09 Math Numb Num Rel	TerraNova 08_09 Math Oper Concept	TerraNova 08_09 Math Prob Solv	TerraNova 08_09 Read Anal Text	TerraNova 08_09 Read Basic Understnd	TerraNova 08_09 Read Eval Ext Mng	TerraNova 08_09 Read Intro Print	TerraNova 08_09 Read Rdg Wrt Strat	TerraNova 08_09 Reading NCE	TerraNova 08_09 Total Score NCE		\r\n"
                                                    +"23364	Tahniyat	Abidi	5	110	Mrs. Cullen	A	F				78	78	67	77	85	87	68	66	77	61	79	71	63	71	66	69	92	66	59	73	77		63	59	66		Delete TRNV Group Data\r\n"
                                                    +"23086	Nicholas	Abramo	5	110	Mrs. Closius	W	M			17	45	61	48	50	68	64	52	48	64	50	68	63	54	58	56	56	86	50	47	64	70		54	49	50		NP = Percentile\r\n"
                                                    +"23676	Devyn	Accardi	5	75	Ms. Blaker	W	F				98	99	96	99	98	99	84	93	97	87	97	93	87	95	99	92	99	93	80	93	92		82	86	99		What is NCE?",
                               UserID= 10,
                               XmlTransform = "<?xml version=\"1.0\" encoding=\"utf-8\"?><transform xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><commonfields><mappings><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Testing Year\" destinationid=\"1\" value=\"2012\" /><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Test Name\" destinationid=\"2\" value=\"test\" /><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Subject\" destinationid=\"3\" value=\"abc\" /><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Subject Ext\" destinationid=\"4\" value=\"ext\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"School 08_09\" sourceposition=\"4\" destination=\"School\" destinationid=\"5\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Grade 08_09\" sourceposition=\"3\" destination=\"Grade\" destinationid=\"6\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Student ID\" sourceposition=\"0\" destination=\"Local ID\" destinationid=\"7\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Student ID\" sourceposition=\"0\" destination=\"SID\" destinationid=\"8\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Last_Name\" sourceposition=\"2\" destination=\"Last Name\" destinationid=\"9\" /></mappings></commonfields><test id=\"1\"><mappings><mapping xsi:type=\"SourceColumnMapping\" source=\"Last_Name\" sourceposition=\"2\" destination=\"-\" destinationid=\"-1\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"-\" sourceposition=\"-1\" destination=\"Total Raw Score\" destinationid=\"15\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Last_Name\" sourceposition=\"2\" destination=\"-\" destinationid=\"-1\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"-\" sourceposition=\"-1\" destination=\"-\" destinationid=\"-1\" /></mappings></test><test id=\"2\"><mappings /></test></transform>"
                           },
                           new MappingInformation
                           { 
                               MapID=3, 
                               LoaderType = MappingLoaderType.DataLoader, 
                               CreatedDate = DateTime.Now, 
                               LastestModifiedDate = DateTime.Now,
                               Name="test mapping 2",
                               ProgressStatus = MappingProgressStatus.InProgress,
                               SourceFileContent = "Student ID	First_Name	Last_Name	Grade 08_09	School 08_09	Homeroom Teacher 08_09	Ethnicity	Gender	LEP	Section 504	Special Education	TerraNova 08_09 Language NP	TerraNova 08_09 Math NP	TerraNova 08_09 Reading NP	TerraNova 08_09 Total Score NP	TerraNova 08_09 Lang Edit Skill	TerraNova 08_09 Lang Sent Struct	TerraNova 08_09 Lang Wrt Strat	TerraNova 08_09 Language NCE	TerraNova 08_09 Math Alg Pattern Func	TerraNova 08_09 Math Communi	TerraNova 08_09 Math Comp Est	TerraNova 08_09 Math Data Stat Prob	TerraNova 08_09 Math Geom Spat Sns	TerraNova 08_09 Math Measurement	TerraNova 08_09 Math NCE	TerraNova 08_09 Math Numb Num Rel	TerraNova 08_09 Math Oper Concept	TerraNova 08_09 Math Prob Solv	TerraNova 08_09 Read Anal Text	TerraNova 08_09 Read Basic Understnd	TerraNova 08_09 Read Eval Ext Mng	TerraNova 08_09 Read Intro Print	TerraNova 08_09 Read Rdg Wrt Strat	TerraNova 08_09 Reading NCE	TerraNova 08_09 Total Score NCE		\r\n"
                                                    +"23364	Tahniyat	Abidi	5	110	Mrs. Cullen	A	F				78	78	67	77	85	87	68	66	77	61	79	71	63	71	66	69	92	66	59	73	77		63	59	66		Delete TRNV Group Data\r\n"
                                                    +"23086	Nicholas	Abramo	5	110	Mrs. Closius	W	M			17	45	61	48	50	68	64	52	48	64	50	68	63	54	58	56	56	86	50	47	64	70		54	49	50		NP = Percentile\r\n"
                                                    +"23676	Devyn	Accardi	5	75	Ms. Blaker	W	F				98	99	96	99	98	99	84	93	97	87	97	93	87	95	99	92	99	93	80	93	92		82	86	99		What is NCE?",
                               UserID= 10,
                               XmlTransform = "<?xml version=\"1.0\" encoding=\"utf-8\"?><transform xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><commonfields><mappings><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Testing Year\" destinationid=\"1\" value=\"2012\" /><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Test Name\" destinationid=\"2\" value=\"test\" /><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Subject\" destinationid=\"3\" value=\"abc\" /><mapping xsi:type=\"FixedValueMapping\" source=\"\" sourceposition=\"-1\" destination=\"Subject Ext\" destinationid=\"4\" value=\"ext\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"School 08_09\" sourceposition=\"4\" destination=\"School\" destinationid=\"5\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Grade 08_09\" sourceposition=\"3\" destination=\"Grade\" destinationid=\"6\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Student ID\" sourceposition=\"0\" destination=\"Local ID\" destinationid=\"7\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Student ID\" sourceposition=\"0\" destination=\"SID\" destinationid=\"8\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Last_Name\" sourceposition=\"2\" destination=\"Last Name\" destinationid=\"9\" /></mappings></commonfields><test id=\"1\"><mappings><mapping xsi:type=\"SourceColumnMapping\" source=\"Last_Name\" sourceposition=\"2\" destination=\"-\" destinationid=\"-1\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"-\" sourceposition=\"-1\" destination=\"Total Raw Score\" destinationid=\"15\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"Last_Name\" sourceposition=\"2\" destination=\"-\" destinationid=\"-1\" /><mapping xsi:type=\"SourceColumnMapping\" source=\"-\" sourceposition=\"-1\" destination=\"-\" destinationid=\"-1\" /></mappings></test></transform>"
                           }
                       };
        }

        public IQueryable<MappingInformation> Select()
        {
            return table.AsQueryable();
        }

        public void Save(MappingInformation item)
        {
            var entity = table.FirstOrDefault(x => x.MapID.Equals(item.MapID));

            if (entity.IsNull())
            {
                item.MapID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<MappingInformation, MappingInformation>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(MappingInformation item)
        {
            var entity = table.FirstOrDefault(x => x.MapID.Equals(item.MapID));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}
