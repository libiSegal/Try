using com.Instant.Mishor.Net.Clients.Settings;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace com.Instant.Mishor.Net.Clients
{
    public class ApiClient
    {
        private readonly ApiSettings? settings;

        private const int Source = 1;

        public ApiClient(ApiSettings settings)
        {
            this.settings = settings;
        }

        public async Task<ResponseObject?> SearchHotels(string datefrom, string dateto, int id, string type = "hotel", string currencies = "USD", int adults = 2)
        {
            var request = new RequestObject()
            {
                CustomerCountry = "IL",
                Currencies = new List<string> { currencies },
                CustomFields = new List<object>(),
                Dates = new DateRangeObject()
                {
                    From = datefrom,//"2019-12-05";
                    To = dateto//"2019-12-05";
                },
                Destinations = new List<DestinationObject>()
                {
                    new DestinationObject()
                    {
                        Id = id,
                        Type = type
                    }
                },
                Filters = new List<FilterObject>()
                {
                    new FilterObject()
                    {
                        Name = "payAtTheHotel",
                        Value  = true
                    },
                    new FilterObject()
                    {
                        Name = "onRequest",
                        Value  = false
                    },
                    new FilterObject()
                    {
                        Name = "showSpecialDeals",
                        Value  = true
                    },
                },
                Pax = new List<PaxObject>()
                {
                    new PaxObject()
                    {
                        Adults = adults
                    }
                },
                Timeout = 20,
                Service = "hotels"
            };

            var session = await GetSession(request).ConfigureAwait(false);
            var response = await GetResponse(request, session!);

            response!.Results!.ForEach(x => x.Source = Source);

            return response;
        }

        private async Task<ResponseObject?> GetResponse(RequestObject request, SessionObject session)
        {
            var errorMessage = string.Empty;
            var json = JsonConvert.SerializeObject(request);

            var restClient = new RestClient($"{settings!.SearchUrl}/hotels/poll/{session.Timestamp}");
            //var request = new RestRequest(Method.POST);
            var restRequest = new RestRequest();

            restRequest.AddHeader("postman-token", "121a7ba3-fa13-4f9d-faa5-2d4322267237");
            restRequest.AddHeader("cache-control", "no-cache");
            restRequest.AddHeader("aether-access-token", settings.Aether!.AccessToken!);
            restRequest.AddHeader("aether-application-key", settings.Aether!.ApplicationKey!);
            restRequest.AddHeader("content-type", "application/json");
            restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);

            //IRestResponse response = CallToServer(client, request);
            //responseastropay = json_serializer.Deserialize<PollHotel>(response);

            var restResponse = await restClient.PostAsync(restRequest).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<ResponseObject>(restResponse.Content!);

            //responseastropay!.results = Allresults;
            response!.JsonSearchHotels = json;
            response!.SearchHotels = request;
            response!.Dates = request.Dates;

            if (response.Results != null)
            {
                foreach (var search in response.Results)
                {
                    try
                    {
                        if (search.Cancellation == null || search.Cancellation.Frames == null || search.Cancellation.Frames.Count == 0)
                        {
                            search.Cancellation = await GetCancellation(request, search.Code!);
                        }
                    }
                    catch
                    {

                        search.Cancellation = null;
                    }
                }
            }

            return response;
        }

        private async Task<SessionObject?> GetSession(RequestObject request)
        {
            var json = JsonConvert.SerializeObject(request, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var restClient = new RestClient(settings!.SearchUrl + "/hotels/search");

            //var request = new RestRequest(Method.POST);
            var restRequest = new RestRequest();

            //  request.AddHeader("cache-control", "no-cache");
            restRequest.AddHeader("aether-access-token", settings.Aether!.AccessToken!);
            restRequest.AddHeader("Aether-application-key", settings.Aether!.ApplicationKey!);
            restRequest.AddHeader("content-type", "application/json");
            restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);           

            //IRestResponse response = CallToServer(client, request);
            var response = await restClient.PostAsync(restRequest).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<SessionObject>(response!.Content!);
        }

        private async Task<CancellationObject?> GetCancellation(RequestObject request, string code)
        {
            var json = JsonConvert.SerializeObject(request);
            var restClient = new RestClient(settings!.SearchUrl + "/hotels/cancellation/" + code);
            //var request = new RestRequest(Method.POST);
            var restRequest = new RestRequest();

            restRequest.AddHeader("cache-control", "no-cache");
            restRequest.AddHeader("aether-access-token", settings.Aether!.AccessToken!);
            restRequest.AddHeader("Aether-application-key", settings.Aether!.ApplicationKey!);
            restRequest.AddHeader("content-type", "application/json");
            restRequest.AddParameter("application/json", json, ParameterType.RequestBody);

            //  IRestResponse response = client.Execute(request);
            //IRestResponse response = CallToServer(client, request);
            //CancellationRes responseastropay = json_serializer.Deserialize<CancellationRes>(response);
            var restResponse = await restClient.PostAsync(restRequest).ConfigureAwait(false);

            var response = JsonConvert.DeserializeObject<CancellationResponseObject>(restResponse.Content!)!;

            if (response.Results!.Any()) return response.Results!.First().Cancellation;

            return null;
        }

        public class CancellationResponseObject
        {
            public int? Timestamp { get; set; }
            public string? RequestTime { get; set; }
            public string? Status { get; set; }
            public int? Completed { get; set; }
            public string? SessionId { get; set; }
            public int? ProcessTime { get; set; }
            public List<ResultObject>? Results { get; set; }
        }

        public class HotelObject
        {
            public string? HotelName { get; set; }
            public int? HotelCode { get; set; }
            public int? CountryId { get; set; }
            public int? CityId { get; set; }
            public string? Location { get; set; }
            public string? Thumbnail { get; set; }
            public double? Longitude { get; set; }
            public double? Latitude { get; set; }
            public List<OfferObject>? Offers { get; set; }
        }

        public sealed class RequestObject
        {
            public List<string>? Currencies { get; set; }

            public string? CustomerCountry { get; set; }

            public List<object>? CustomFields { get; set; }

            public DateRangeObject? Dates { get; set; }

            public List<DestinationObject>? Destinations { get; set; }

            public List<FilterObject>? Filters { get; set; }

            public List<PaxObject>? Pax { get; set; }

            public int Timeout { get; set; }

            public string? Service { get; set; }
        }

        public sealed class ResponseObject
        {
            public RequestObject? SearchHotels { get; set; }

            public string? JsonSearchHotels { get; set; }

            public int Timestamp { get; set; }

            public string? RequestTime { get; set; }

            public string? Status { get; set; }

            public int Completed { get; set; }

            public string? SessionId { get; set; }

            public DateRangeObject? Dates { get; set; }

            public int ProcessTime { get; set; }

            public ErrorObject? Error { get; set; }

            public List<ResultObject>? Results { get; set; }
        }

        public class ErrorObject
        {
            public int Code { get; set; }
            public string? Message { get; set; }
        }

        public sealed class SessionObject
        {
            public int Timestamp { get; set; }
            public string? RequestTime { get; set; }
            public string? Status { get; set; }
            public int Completed { get; set; }
            public string? SessionId { get; set; }
            public int ProcessTime { get; set; }
        }

        public sealed class ResultObject
        {
            public PriceObject? Price { get; set; }
            public NetPriceObject? NetPrice { get; set; }
            public BarRateObject? BarRate { get; set; }
            public string? Confirmation { get; set; }
            public string? PaymentType { get; set; }
            public bool? PackageRate { get; set; }
            public bool? Commissionable { get; set; }
            public List<ProviderObject>? Providers { get; set; }
            public List<object>? SpecialOffers { get; set; }
            public List<ItemObject>? Items { get; set; }
            public CancellationObject? Cancellation { get; set; }
            public string? Code { get; set; }
            public DateRangeObject? Dates { get; set; }
            public int? Source { get; set; }
            public OfferObject? Offer { get; set; }
        }


        public sealed class OfferObject
        {
            public string? HotelSearchCode { get; set; }
            public string? CxlDeadLine { get; set; }
            public bool? NonRef { get; set; }
            public List<string>? Rooms { get; set; }
            public string? RoomBasis { get; set; }
            public int? Availability { get; set; }
            public int? TotalPrice { get; set; }
            public string? Currency { get; set; }
            public string? Category { get; set; }
            public string? Remark { get; set; }
            public string? Special { get; set; }
            public bool? Preferred { get; set; }
            public int? Hotelcode { get; set; }
            public int? Adults { get; set; }

            public string? CategorySystem { get; set; } = "Standard";

            public string? Bedding { get; set; } = "Double";

            public CancellationObject? Cancellation { get; set; }

            public string? RequestJson { get; set; }

            public string? ResponseJson { get; set; }

            public int? OpportunityId { get; set; }

            private ResultObject HotelToResult(HotelObject hotel, DateRangeObject dates, int source)
            {
                var result = new ResultObject()
                {
                    Source = source,
                    Items = new List<ItemObject>()
                    {
                        new ItemObject()
                        {
                            HotelId = hotel.HotelCode.ToString(),
                            Category = CategorySystem,
                            Bedding = Bedding,
                            Board = RoomBasis,
                            Pax = new PaxObject()
                            {
                                Adults = Adults
                            }
                        }
                    },
                    Dates = dates,
                    Code = HotelSearchCode,
                    Price = new PriceObject()
                    {
                        Amount = TotalPrice,
                        Currency = Currency
                    },
                    Providers = new List<ProviderObject>()
                    {
                        new ProviderObject()
                        {
                            Id = 2,
                            Name = "GoGlobal"
                        }
                    },
                    Cancellation = new CancellationObject()
                    {
                        Type = NonRef.GetValueOrDefault() ? "none-refundable" : "fully-refundable",
                        Frames = new List<FrameObject>()
                    }
                };

                var frame = new FrameObject();

                if (!NonRef.GetValueOrDefault())
                {
                    frame.From = DateTime.Now.ToString();
                    frame.To = DateTime.Parse(CxlDeadLine!).ToString();
                    frame.Penalty = new PenaltyObject();
                    frame.Penalty.Amount = 0;
                    frame.Penalty.Currency = Currency;
                }
                else
                {

                    frame.From = DateTime.Parse(CxlDeadLine!).ToString();
                    frame.To = DateTime.Parse(CxlDeadLine!).ToString();
                    frame.Penalty = new PenaltyObject();
                    frame.Penalty.Amount = TotalPrice;
                    frame.Penalty.Currency = Currency;
                }

                result.Cancellation.Frames.Add(frame);

                this.Cancellation = result.Cancellation;

                return result;
            }

        }

        public class CancellationObject
        {
            public string? Type { get; set; }
            public List<FrameObject>? Frames { get; set; }
        }

        public sealed class ItemObject
        {
            public string? Name { get; set; }
            public string? Category { get; set; }
            public string? Bedding { get; set; }
            public string? Board { get; set; }
            public string? HotelId { get; set; }
            public PaxObject? Pax { get; set; }
            public QuantityObject? Quantity { get; set; }
        }

        public sealed class QuantityObject
        {
            public int? Min { get; set; }
            public int? Max { get; set; }
        }

        public sealed class DateRangeObject
        {
            public string? From { get; set; }
            public string? To { get; set; }
        }

        public sealed class DestinationObject
        {
            public int? Id { get; set; }
            public string? Type { get; set; }
        }

        public sealed class FilterObject
        {
            public string? Name { get; set; }
            public bool Value { get; set; }
        }

        public sealed class PaxObject
        {
            public int? Adults { get; set; }
        }

        public class PenaltyObject
        {
            public float? Amount { get; set; }
            public string? Currency { get; set; }
        }

        public class FrameObject
        {
            public string? From { get; set; }
            public string? To { get; set; }
            public PenaltyObject? Penalty { get; set; }
        }

        public class BarRateObject
        {
            public int? Amount { get; set; }
            public string? Currency { get; set; }
        }

        public class NetPriceObject
        {
            public double Amount { get; set; }
            public string? Currency { get; set; }
        }

        public class PriceObject
        {
            public float? Amount { get; set; }
            public string? Currency { get; set; }
        }

        public class ProviderObject
        {
            public int? Id { get; set; }
            public string? Name { get; set; }
        }
    }
}