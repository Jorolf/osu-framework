// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using osu.Framework.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace osu.Framework.IO.Stores
{
    public class LocaleStore : IResourceStore<string>
    {
        private readonly IResourceStore<byte[]> store;
        private readonly Func<string, string> localePath;

        private readonly IDictionary<string,string> locales;
        public IEnumerable<string> Locales => locales?.Keys;
        public string GetDisplayName(string locale) => locales[locale];

        private IDictionary<string, string> localisation;
        private Bindable<string> locale = new Bindable<string>();
        public Bindable<string> Locale
        {
            get
            {
                return locale;
            }
            set
            {
                locale.ValueChanged -= loadLocale;
                locale = value;
                locale.ValueChanged += loadLocale;
            }
        }
        public string FallbackLocale;


        public LocaleStore(IResourceStore<byte[]> store, Func<string,string> localePath)
        {
            this.store = store;
            this.localePath = localePath;

            locale.ValueChanged += loadLocale;

            locales = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(store.GetStream(localePath("locales"))))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split('=');
                    locales.Add(data[0], data[1]);
                }
            }
            FallbackLocale = Locales.First();
            Locale.Value = FallbackLocale;
        }

        public string Get(string name)
        {
            return name;
        }

        public Stream GetStream(string name)
        {
            throw new NotImplementedException();
        }

        private void loadLocale(string locale)
        {
            localisation = new Dictionary<string, string>();
            using (StreamReader reader = new StreamReader(store.GetStream(localePath(locale))))
            {
                while (!reader.EndOfStream)
                {
                    string[] data = reader.ReadLine().Split('=');
                    localisation.Add(data[0], data[1]);
                }
            }
        }
    }
}
