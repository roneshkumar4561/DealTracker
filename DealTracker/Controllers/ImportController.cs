using DealTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace DealTracker.Controllers
{
    public class ImportController : Controller
    {
        //Adding the Nlog variable for loggin purposes.
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);
                    string[] seps = { "\",", ",\"" };
                    char[] quotes = { '\"', ' ' };

                    //Validate uploaded file and return error.
                    if (fileExtension != ".csv")
                    {
                        ViewBag.Message = "Please select the file with .csv extension";
                        return View();
                    }

                    var dealList = new List<Deals>();
                    using (var reader = new StreamReader(postedFile.InputStream))
                    {
                        //Get the header row
                        string[] headers = reader.ReadLine().Split(',');

                        //Loop through the records
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var lineNoDoubleQuotes = line.Replace("\"", "");

                            var indexSecondComma = IndexNthCharacter(line, ',', 2);

                            //Checking for first pattern i.e. rows 5th and 10th in CSV file
                            if (line[indexSecondComma+1] == '"')
                            {
                                dealList.Add(new Deals
                                {
                                    DealNumber = int.Parse(lineNoDoubleQuotes.Substring(0, IndexNthCharacter(lineNoDoubleQuotes, ',', 1))),
                                    CustomerName = lineNoDoubleQuotes.Substring(IndexNthCharacter(lineNoDoubleQuotes, ',', 1) + 1, (IndexNthCharacter(lineNoDoubleQuotes, ',', 2) - IndexNthCharacter(lineNoDoubleQuotes, ',', 1)) - 1),
                                    DealershipName = lineNoDoubleQuotes.Substring(IndexNthCharacter(lineNoDoubleQuotes, ',', 2) + 1, (IndexNthCharacter(lineNoDoubleQuotes, ',', 4) - IndexNthCharacter(lineNoDoubleQuotes, ',', 2)) - 1),
                                    Vehicle = lineNoDoubleQuotes.Substring(IndexNthCharacter(lineNoDoubleQuotes, ',', 4) + 1, (IndexNthCharacter(lineNoDoubleQuotes, ',', 5) - IndexNthCharacter(lineNoDoubleQuotes, ',', 4)) - 1),
                                    Price = double.Parse(lineNoDoubleQuotes.Substring(IndexNthCharacter(lineNoDoubleQuotes, ',', 5) + 1, (IndexNthCharacter(lineNoDoubleQuotes, ',', 7) - IndexNthCharacter(lineNoDoubleQuotes, ',', 5) - 1))),
                                    Date = lineNoDoubleQuotes.Substring(IndexNthCharacter(lineNoDoubleQuotes, ',', 7) + 1, (lineNoDoubleQuotes.Length - IndexNthCharacter(lineNoDoubleQuotes, ',', 7) - 1))
                                });
                            }
                            //Checking for second pattern i.e. rest of the rows in CSV file
                            else
                            {
                                //Two levels of filters for dealing with special characters in Price field
                                var allFields = line
                                       .Split(seps, StringSplitOptions.None)
                                       .Select(s => s.Trim(quotes).Replace("\\\"", "\""))
                                       .ToArray();

                                var dealCustDealershipVehicle = allFields[0].Split(',');

                                dealList.Add(new Deals
                                {
                                    DealNumber = int.Parse(dealCustDealershipVehicle[0].ToString()),
                                    CustomerName = dealCustDealershipVehicle[1].ToString(),
                                    DealershipName = dealCustDealershipVehicle[2].ToString(),
                                    Vehicle = dealCustDealershipVehicle[3].ToString(),
                                    Price = double.Parse(allFields[1].ToString().Replace(",", "")),
                                    Date = allFields[2].ToString()
                                });
                            }

                            ViewBag.MostSoldVehicle = dealList.OrderByDescending(p => p.Price).First().Vehicle;
                        }
                    }

                    return View("View", dealList);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error while parsing the CSV file";
                    logger.Log(LogLevel.Info, ex.Message);
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }

        //Function for getting the index of the Nth occurance of the given character in a string
        private int IndexNthCharacter(string input, char ch, int n)
        {
            var result = input
              .Select((c, i) => new { c, i })
              .Where(x => x.c == ch)
              .Skip(n - 1)
              .FirstOrDefault();

            return result != null ? result.i : -1;
        }
    }
}