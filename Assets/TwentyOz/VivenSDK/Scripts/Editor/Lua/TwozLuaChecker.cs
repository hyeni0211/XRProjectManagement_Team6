using System.Collections;
using System.Reflection;
using System.Text;
using TwentyOz.VivenSDK.Scripts.Core.Lua;
using UnityEngine;

namespace TwentyOz.VivenSDK.Scripts.Editor.Lua
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    static class LuaParser
    {
        // Regex patterns to identify global variables and functions
        private static readonly Regex globalVariableRegex = new Regex(@"^\s*(\w+)\s*=\s*.*$", RegexOptions.Multiline);
        private static readonly Regex globalFunctionRegex = new Regex(@"^\s*function\s+(\w+)\s*\(.*\)\s*$", RegexOptions.Multiline);
        private static readonly Regex multiLineCommentRegex = new Regex(@"--\[\[.*?\]\]", RegexOptions.Singleline);
        private static readonly Regex descriptionRegex = new Regex(@"^[\s]*---@.+", RegexOptions.Multiline);
        
        private static List<string> globalVariables = new List<string>();
        private static List<string> globalFunctions = new List<string>();


        public static void ParseLuaScript(ref string luaScript)
        {
            luaScript = RemoveMultiLineComments(luaScript);

            // Parse global variables, functions, and their descriptions
            var globalEntities = ParseGlobalsWithDescriptions(luaScript);
            
            
            // Print global variables and functions with descriptions
            foreach (var entity in globalEntities)
            {
                if (entity.Type == "Variable")
                {
                    // Parse Description
                    // ---@type Type Description
                    var varType = "";
                    var varDescription = "";
                    var varTypeMatch = Regex.Match(entity.Description, @"---@type\s+(\w+)");
                    if (varTypeMatch.Success)
                    {
                        varType = varTypeMatch.Groups[1].Value;
                    }
                    var varDescriptionMatch = Regex.Match(entity.Description, @"---@type\s+\w+\s+(.+)");
                    if (varDescriptionMatch.Success)
                    {
                        varDescription = varDescriptionMatch.Groups[1].Value;
                    }
                    // Print global variable in Annotation Form
                    // ---@field name type description
                    Debug.Log($"---@field {entity.Name} {varType} {varDescription}");
                    globalVariables.Add($"---@field {entity.Name} {varType} {varDescription}");
                }
                else if (entity.Type == "Function")
                {
                    // Parse Description
                    // ---@param Type Name Description\n...\n---@return Type Description
                    var funcParams = "";
                    var funcDescription = "";
                    
                    // Find All Params
                    var funcParamsMatch = Regex.Matches(entity.Description, @"---@param\s+(.+)");
                    if (funcParamsMatch.Count > 0)
                    {
                        foreach (Match match in funcParamsMatch)
                        {
                            funcParams += match.Groups[1].Value + " ";
                        }
                    }
                    
                    var funcReturnMatch = Regex.Match(entity.Description, @"---@return\s+(.+)");
                    if (funcReturnMatch.Success)
                    {
                        funcDescription = funcReturnMatch.Groups[1].Value;
                    }
                    
                    // Print global function in Annotation Form
                    // ---@param name1 type1
                    // ---@param name2 type2
                    // ---@return retName Type
                    // function FunctionName(name1, name2) end
                    
                    string paramsBuilder = "";
                    string[] paramsSplit = funcParams.Split(' ');
                    if (paramsSplit.Length % 3 == 0)
                    {
                        for (int i = 0; i < paramsSplit.Length; i += 3)
                        {
                            paramsBuilder += $"---@param {paramsSplit[i+1]} {paramsSplit[i]}\n";
                        }
                    }

                    string returnType = "";
                    string returnDescription = "";
                    string[] returnSplit = funcDescription.Split(' ');
                    if (returnSplit.Length >= 2)
                    {
                        returnType = returnSplit[0];
                        returnDescription = returnSplit[1];
                    }

                    Debug.Log($"{paramsBuilder}---@return {returnType} {returnDescription}\nfunction {entity.Name}() end");
                    globalFunctions.Add($"{paramsBuilder}---@return {returnType} {returnDescription}\nfunction {entity.Name}() end");
                }
                // Debug.Log($"Name: {entity.Name}");
                // if (!string.IsNullOrEmpty(entity.Description))
                // {
                //     Debug.Log($"Description: {entity.Description}");
                // }
                // Debug.Log($"Type: {entity.Type}\n");
            }
        }

        private static string RemoveMultiLineComments(string luaScript)
        {
            return multiLineCommentRegex.Replace(luaScript, string.Empty);
        }

        private static List<GlobalEntity> ParseGlobalsWithDescriptions(string luaScript)
        {
            var entities = new List<GlobalEntity>();
            var lines = luaScript.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> currentDescription = new List<string>();

            foreach (var line in lines)
            {
                // Check if the line is a description line
                if (descriptionRegex.IsMatch(line))
                {
                    currentDescription.Add(line.Trim());
                }
                else
                {
                    // Check if the line is a global variable
                    var variableMatch = globalVariableRegex.Match(line);
                    if (variableMatch.Success)
                    {
                        entities.Add(new GlobalEntity
                        {
                            Name = variableMatch.Groups[1].Value,
                            Description = string.Join("\n", currentDescription),
                            Type = "Variable"
                        });
                        currentDescription.Clear();
                        continue;
                    }

                    // Check if the line is a global function
                    var functionMatch = globalFunctionRegex.Match(line);
                    if (functionMatch.Success)
                    {
                        entities.Add(new GlobalEntity
                        {
                            Name = functionMatch.Groups[1].Value,
                            Description = string.Join("\n", currentDescription),
                            Type = "Function"
                        });
                        currentDescription.Clear();
                    }
                    // local Variable이나 local Function의 Annotation이라면 Flush
                    currentDescription.Clear();
                }
            }

            return entities;
        }

        private class GlobalEntity
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
        }

        // Basic keyword check to avoid Lua reserved words being marked as globals
        private static bool IsLuaKeyword(string word)
        {
            var luaKeywords = new HashSet<string>
            {
                "and", "break", "do", "else", "elseif", "end", "false", "for", "function",
                "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true",
                "until", "while"
            };

            return luaKeywords.Contains(word);
        }
    }



    public static class TwozLuaChecker
    {
        public static void GenerateMetaFile(VivenLuaBehaviour vivenLuaBehaviour)
        {
            VivenScript script = vivenLuaBehaviour.luaScript;
            Injection injections = vivenLuaBehaviour.injection;

            StreamWriter metaFile;
            // Find Existing Meta File
            // if not exist, create new one
            var dir = "LuaAutoCompletion/VivenScriptGenerated/";
            

            // Dictionary<string, string> globalVariables = new Dictionary<string, string>();
            // Dictionary<string, string> injectedVariables = new Dictionary<string, string>();
            //
            
            Debug.Log("Start Parse ..." + script.name);
            
            // Generate Global Variables
            LuaParser.ParseLuaScript(ref script.scriptString);
            
            if (Directory.Exists(dir))
            {
                // Load Existing Meta File
                metaFile = new StreamWriter(Path.Join(dir, script.name + ".def.lua"), true, Encoding.UTF32);
            }
            else
            {
                // Create New Meta File
                Directory.CreateDirectory(dir);

                metaFile = new StreamWriter(Path.Join(dir, script.name + ".def.lua"), false, Encoding.UTF32);

                // Generate New Meta File
                metaFile.WriteLine("---@meta\n");
                metaFile.WriteLine("---@class " + script.name);
                // foreach (var variable in globalVariables)
                // {
                //     metaFile.WriteLine("---@field " + variable.Key + " " + variable.Value);
                //     
                // }
                metaFile.WriteLine("local " + script.name + " = {}\n");
            }
            
            Debug.Log("Parse Done");
            
            
            // Check If Meta File is Valid
            /////

            metaFile.Flush();
            metaFile.Close();
        }

        private static void ParseLuaScript(ref StreamWriter metaFile)
        {
            // Parse Lua Script and Find all Global Variables
            
            
        }

        public static bool Check(VivenLuaBehaviour vivenLuaBehaviour)
        {
            if (vivenLuaBehaviour.luaScript == null)
            {
                Debug.LogError("vivenLuaBehaviour is null");
                return false;
            }
            VivenScript script = vivenLuaBehaviour.luaScript;
            Injection injections = vivenLuaBehaviour.injection;
            
            // CheckInjections
            if (FindMissingOrNullInjections(injections))
            {
                return false;
            }

            // lua 스크립트에서 사용하는 injected variable이 injection list에 있는 지 확인
            if (!ValidateInjectedVariableInScript(script, injections))
                return false;
            
            // lua check를 사용한 lua script 정적 분석 
            AnalyzeLuaScriptWithLuaCheck(script, injections);
            
            return true;
        }


        private static bool FindMissingOrNullInjections(Injection injections)
        {
            bool isNullOrMissingFound = false;
            if (injections == null)
            {
                Debug.Log("injections is null"); 
                return false;
            }
            Type myType = injections.GetType();

            IList<FieldInfo> fields = new List<FieldInfo>(myType.GetFields());

            //Debug.Log(fields.Count);
            
            foreach(var fieldInfo in fields)
            {
                // if (fieldInfo.FieldType.IsArray)
                // {
                //     Debug.Log("Array Type : :"  + fieldInfo.FieldType.ToString());
                // }
                // else
                // {
                //     Debug.Log("Not Array Type : "  + fieldInfo.FieldType.ToString());
                // }
                Type ArrayType = fieldInfo.FieldType.GetElementType();
                var injectionVarArray = fieldInfo.GetValue(injections);
                
                // Debug.Log("Array Type : " + ArrayType);

                IList<FieldInfo> fieldFields = new List<FieldInfo>(ArrayType.GetFields());
                // foreach (var fieldField in fieldFields)
                // {
                //     Debug.Log("Field Field : " + fieldField);
                // }
                
                var valueField = ArrayType.GetField("value");
                // Debug.Log("value Type : " + field.FieldType);
                var nameField = ArrayType.GetField("name");
                foreach (var injectedVar in (IEnumerable)injectionVarArray)
                {
                    object value = valueField.GetValue(injectedVar);
                    object name = nameField.GetValue(injectedVar);
                    // Debug.Log("Name : " + name + " Value : " + value);
                    
                    // value가 missing이면  value == Exception of type 'UnityEngine.MissingReferenceException' was thrown (active: Exception of type 'UnityEngine.MissingReferenceException' was thrown, layer: Exception of type 'UnityEngine.MissingReferenceException' was thrown)
                    // ExceptionThrown 되었을 때 따로 값 처리 어케 할 지 모르겠음...
                    // value 가 None이면  value == null
                    if (object.ReferenceEquals(value, null) || value.ToString().Equals("null"))
                    {
                        Debug.LogError("injected value is null: " + name.ToString());
                        isNullOrMissingFound = true;
                    }
                }
            }
            return isNullOrMissingFound;
        }
        
        private static bool ValidateInjectedVariableInScript(VivenScript script, Injection injections)
        {
            // find name from script
            return true;
        }
        
        private static void AnalyzeLuaScriptWithLuaCheck(VivenScript script, Injection injections)
        {
            //throw new NotImplementedException();
        }
    }
}