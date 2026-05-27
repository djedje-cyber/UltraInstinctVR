using System;
using System.Collections.Generic;




    [AttributeUsage(AttributeTargets.Method)]
    public class SensorAttribute : Attribute
    {
        public string ClassName { get; }
        public List<ParamData> Params { get; }
        public SensorAttribute(string className)
        {
            ClassName = className;
            Params = new List<ParamData>();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class EffectorAttribute : Attribute
    {
        public string ClassName { get; }
        public List<ParamData> Params { get; }
        public EffectorAttribute(string className)
        {
            ClassName = className;
            Params = new List<ParamData>();
        }
    }