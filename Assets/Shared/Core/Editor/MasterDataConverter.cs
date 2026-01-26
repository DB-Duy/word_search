// using System.Collections.Generic;
// using System.Data;
// using System.IO;
// using ExcelDataReader;
// using Newtonsoft.Json;
// using UnityEditor;
// using UnityEngine;
//
// namespace Shared.Core.Editor
// {
//     public class MasterDataConverter
//     {
//         public static string excelFilePath = "MasterData/ng_game_config.xlsx"; // Place your Excel file in the StreamingAssets folder
//
//         [MenuItem("Shared/MasterData/Update")]
//         private static void PrintProjectRootPath()
//         {
//             ReadExcelInEditor();
//         }
//         
//         public static void ReadExcelInEditor()
//         {
//
//             var projectPath = Path.GetFullPath(Application.dataPath + "/..");
//             string filePath =  Path.Combine(projectPath, excelFilePath);
//
//             if (!File.Exists(filePath))
//             {
//                 Debug.LogError("Excel file not found: " + filePath);
//                 return;
//             }
//
//             // Open the Excel file
//             using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
//             {
//                 // Register CodePagesEncodingProvider for compatibility
//                 // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
//
//                 using (var reader = ExcelReaderFactory.CreateReader(stream))
//                 {
//                     // Convert to DataSet for easier handling
//                     var result = reader.AsDataSet();
//
//                     foreach (DataTable table in result.Tables)
//                     {
//                         Debug.Log($"Reading sheet: {table.TableName}");
//                         var keys = new List<string>();
//                         for (int col = 0; col < table.Columns.Count; col++)
//                         {
//                             var k = table.Rows[0][col].ToString().Trim().ToLower().Replace(" ", "_"); 
//                             if (string.IsNullOrEmpty(k)) break;
//                             keys.Add(k);
//                         }
//                         var dict = new Dictionary<string, Dictionary<string, object>>();
//                         for (int row = 1; row < table.Rows.Count; row++)
//                         {
//                             var id = table.Rows[row][0];
//                             Dictionary<string, object> rowData = new();
//                             for (int col = 0; col < keys.Count; col++)
//                             {
//                                 rowData.Add(keys[col], table.Rows[row][col]);
//                             }
//                             dict.Add(id.ToString(), rowData);
//                             Debug.Log(rowData);
//                         }
//
//                         var jsonString = JsonConvert.SerializeObject(dict, Formatting.Indented);
//                         Debug.Log(jsonString);
//                     }
//                 }
//             }
//         }
//
//     }
// }