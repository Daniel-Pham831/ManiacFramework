using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;

namespace Maniac.Utils.Extension
{
    public static class TypeExtension
    {
        static Dictionary<Type, Delegate> _cachedIL = new Dictionary<Type, Delegate>();
        
        public static IEnumerable<Type> GetAllSubclasses(this Type baseType, List<Type> excepList = null)
        {
            if (excepList == null)
            {
                excepList = new List<Type>();
            }

            return System.Reflection.Assembly.GetAssembly(baseType).GetTypes().Where(type => type.IsSubclassOf(baseType) && !excepList.Contains(type));
        }
        
        public static bool IsSameOrSubclass(this Type potentialDescendant,Type potentialBase)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        public static T CloneBySerialization<T>(this T classToClone) where T : class
        {
            string clonedJson = JsonConvert.SerializeObject(classToClone);
            return JsonConvert.DeserializeObject(clonedJson, typeof(T)) as T;
        }
        
        public static T CloneIL<T>(this T myObject)
        {
            Delegate myExec = null;
            var type = myObject.GetType();
            if (!_cachedIL.TryGetValue(type, out myExec))
            {
                // Create ILGenerator
                DynamicMethod dymMethod = new DynamicMethod("DoClone", typeof(T), new Type[] { typeof(T) }, true);
                ConstructorInfo cInfo = type.GetConstructor(new Type[] { });

                ILGenerator generator = dymMethod.GetILGenerator();

                LocalBuilder lbf = generator.DeclareLocal(typeof(T));
                //lbf.SetLocalSymInfo("_temp");

                generator.Emit(OpCodes.Newobj, cInfo);
                generator.Emit(OpCodes.Stloc_0);
                foreach (FieldInfo field in type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                {
                    // Load the new object on the eval stack... (currently 1 item on eval stack)
                    generator.Emit(OpCodes.Ldloc_0);
                    // Load initial object (parameter)          (currently 2 items on eval stack)
                    generator.Emit(OpCodes.Ldarg_0);
                    // Replace value by field value             (still currently 2 items on eval stack)
                    generator.Emit(OpCodes.Ldfld, field);
                    // Store the value of the top on the eval stack into the object underneath that value on the value stack.
                    //  (0 items on eval stack)
                    generator.Emit(OpCodes.Stfld, field);
                }
            
                // Load new constructed obj on eval stack -> 1 item on stack
                generator.Emit(OpCodes.Ldloc_0);
                // Return constructed object.   --> 0 items on stack
                generator.Emit(OpCodes.Ret);

                myExec = dymMethod.CreateDelegate(typeof(Func<T, T>));
                _cachedIL.Add(type, myExec);
            }
            return ((Func<T, T>)myExec)(myObject);
        }
    }
}