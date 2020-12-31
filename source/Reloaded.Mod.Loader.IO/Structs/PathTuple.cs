﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reloaded.Mod.Loader.IO.Config;

namespace Reloaded.Mod.Loader.IO.Structs
{
    /// <summary>
    /// A tuple class which stores a string and a config type.
    /// </summary>
    public class PathTuple<TGeneric> where TGeneric : IConfig<TGeneric>, new()
    {
        /// <summary>
        /// The file path to the object.
        /// </summary>
        public string Path      { get; set; }

        /// <summary>
        /// The object in question.
        /// </summary>
        public TGeneric Config  { get; set; }

        public PathTuple(string path, TGeneric o)
        {
            Path = path;
            Config = o;
        }

        /// <summary>
        /// Saves the current object to file.
        /// </summary>
        public void Save() => IConfig<TGeneric>.ToPath(Config, Path);

        /// <summary>
        /// Saves the current object to file asynchronously.
        /// </summary>
        public Task SaveAsync() => IConfig<TGeneric>.ToPathAsync(Config, Path);

        /* Autogenerated by R# */
        protected bool Equals(PathTuple<TGeneric> other)
        {
            return Path == other.Path && EqualityComparer<TGeneric>.Default.Equals(Config, other.Config);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathTuple<TGeneric>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Path, Config);
        }
    }
}