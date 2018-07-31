using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing;

namespace AirshowAddmin
{
    class Helper
    {
        public static bool ValidateEntry(Entry toCheck)
        {
            return (toCheck.Text != null && toCheck.Text.Trim().Length > 0 && toCheck.Text.Trim().Length < 64);
        }

        public static bool ValidateEditor(Editor toCheck)
        {
            return (toCheck.Text != null && toCheck.Text.Trim().Length > 0);
        }

        public static bool ValidateImageFromEntry(Entry EntryToCheck)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EntryToCheck.Text.Trim());

                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException) { return false; }
            catch { return false; }
        }
        public static bool ValidatePickerSelected(Picker pckToCheck)
        {
            return pckToCheck.SelectedIndex != -1;
        }

        public static bool validateCoordinateFromEntry(Entry entry)
        {
            return double.TryParse(entry.Text.Trim(), out double result) ? true : false;
        }

        public static bool ValidateLink(Entry toCheck)
        {
            Regex urlRegex = new Regex("^(http://www.|https://www.|http://|https://)?[a-z0-9]+([-.]{1}[a-z0-9]+)*.[a-z]{2,5}(:[0-9]{1,5})?(/.*)?$");
            return urlRegex.IsMatch(toCheck.Text.Trim());
        }
    }
}
