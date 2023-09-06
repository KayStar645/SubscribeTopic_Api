﻿using Sieve.Models.Interface;

namespace Sieve.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SieveAttribute : Attribute, ISievePropertyMetadata
    {
        /// <summary>
        /// Override name used 
        /// </summary>
        public string Name { get; set; }

        public string FullName => Name;

        public bool CanFilter { get; set; }
        public bool CanSort { get; set; }
    }
}
