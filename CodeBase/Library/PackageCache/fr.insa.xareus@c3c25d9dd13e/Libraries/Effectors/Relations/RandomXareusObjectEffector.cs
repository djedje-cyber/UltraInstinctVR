using System;
using System.Collections.Generic;
using System.Linq;

using Xareus.Relations.Unity;
using Xareus.Scenarios.Context;
using Xareus.Scenarios.Unity;
using Xareus.Scenarios.Unity.Providers;
using Xareus.Scenarios.Utilities;

namespace Xareus.Scenarios.Relations
{
    [Renamed("SEVEN.RandomFiveObjectEffector", "Assembly-CSharp")]
    [Renamed("SEVEN.FIVE.RandomFiveObjectEffector", "Assembly-CSharp")]
    [Renamed("Xareus.Scenarios.Relations.RandomXareusObjectEffector", "Xareus.Unity.Librairies")]
    public class RandomXareusObjectEffector : AUnityEffector
    {
        #region Classes

        /// <summary>
        /// Sets the result of the arithmetical operation.
        /// </summary>
        protected class Result
        {
            #region Fields

            private readonly string path;
            private readonly ContextHolder Contexts;

            #endregion

            #region Constructors

            public Result(string path, ContextHolder contexts)
            {
                this.path = path;
                Contexts = contexts;
            }

            #endregion

            #region Methods

            public void Set(string value)
            {
                ValueParser.SetValue<string>(path, value, Contexts);
            }

            #endregion
        }

        #endregion

        #region Fields

        /// <summary>
        ///
        /// </summary>
        [ProvideConstraint(typeof(XUType))]
        [ConfigurationParameter("type", "The Type of the objects to select from", Necessity.Required)]
        public Type type;

        /// <summary>
        /// The second operand
        /// </summary>
        [ConfigurationParameter("variableName", "The variable in which the selected Object will be stored", Necessity.Required)]
        public string variableName;

        private readonly System.Random rand = new();
        private Result result;
        private List<XUObject> objects = new();

        #endregion

        #region Constructors

        protected RandomXareusObjectEffector(Event @event, Dictionary<string, Parameter> parameters, ContextHolder contexts)
            : base(@event, parameters, contexts) { }

        #endregion

        #region Methods

        public override void SafeReset()
        {
            result = new Result(variableName, Contexts);

            IEnumerable<XUType> typess = from obj in XUObject.FindObjectsOfType(type) select obj as XUType;
            objects = (from types in typess select types.GetComponent<XUObject>()).ToList();
        }

        public override void SafeEffectorUpdate()
        {
            string selectedId = objects[rand.Next(objects.Count)].id;
            result.Set(selectedId);
        }

        #endregion
    }
}
