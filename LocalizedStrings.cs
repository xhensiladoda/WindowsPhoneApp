﻿using AppSDEM.Resources;

namespace AppSDEM
{
    /// <summary>
    /// Fornisce l'accesso alle risorse stringa.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}