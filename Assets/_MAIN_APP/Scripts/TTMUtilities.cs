using System;
using System.Collections.Generic;
using _MAIN_APP.Scripts.Enums;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.Events;

namespace _MAIN_APP.Scripts
{
    public static class TtmUtilities
    {
        internal static void SetupDropdownList(ref TMP_Dropdown dropdownFilter, List<ECategories> tags,
            [CanBeNull] string[] additionalOptions,
            [CanBeNull] UnityAction<int> onFilterChange)
        {
            dropdownFilter.options.Clear();

            // setup filter list
            if (onFilterChange != null)
                dropdownFilter.onValueChanged.AddListener(onFilterChange);

            foreach (var categories in tags)
            {
                // setup dropdown options based on the filter
                dropdownFilter.options.Add(new TMP_Dropdown.OptionData(categories.ToString()));
            }

            dropdownFilter.options.Sort((a, b) => string.Compare(a.text, b.text, StringComparison.Ordinal));

            //Add extra options
            if (additionalOptions != null)
                for (int i = 0; i < additionalOptions.Length; i++)
                {
                    dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData(additionalOptions[i]));
                }

            dropdownFilter.options.Insert(0, new TMP_Dropdown.OptionData("Show All"));
            dropdownFilter.captionText.text = "Show All";
            dropdownFilter.value = 0;
        }
    }
}