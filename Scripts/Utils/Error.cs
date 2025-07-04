using System;
using TribesOfDust.Utils.Misc;

namespace TribesOfDust
{
    public static class Error
    {
        #region Collections

        public static ArgumentException CantAddDuplicate(string arg, object target) => new($"Couldn't add {arg} to {target}, duplicate entry.", arg);
        public static ArgumentException CantAdd(string arg, object target) => new($"Couldn't add {arg} to {target}.", arg);
        public static ArgumentException CantRemove(string arg, object target) => new($"Couldn't remove {arg} from {target}.", arg);
        public static ArgumentException CantFind(string arg, object target) => new($"Couldn't find {arg} in {target}.", arg);

        public static InvalidOperationException InvalidEmpty(object target) => new($"Couldn't perform operation, {target} is empty.");
        public static InvalidOperationException InvalidFull(object target) => new($"Couldn't perform operation, {target} is full.");

        #endregion
        #region Switch

        public static NotImplementedException NotImplementedFor(object target) => new($"Case not implemented for {target}.");
        public static NotImplementedException NotImplementedFor<T>(object target) => new($"Case not implemented for {target} in {typeof(T).Name}.");

        #endregion

        public static GodotException Wrap(Godot.Error error) => new(error);
    }
}