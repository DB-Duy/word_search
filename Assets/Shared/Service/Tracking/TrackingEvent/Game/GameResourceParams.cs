using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class GameResourceParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "game_resource";

        [JsonProperty("type")] public GameResourceAction Type { get; }
        [JsonProperty("currency_name")] public CurrencyName CurrencyName { get; }
        [JsonProperty("currency_amount")] public long CurrencyAmount { get; }
        [JsonProperty("exchange_item")] public ExchangeItem ExchangeItem { get; }
        [JsonProperty("source")] public EventSource Source { get; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public static GameResourceParams Spent(CurrencyName currencyName, long currencyAmount, EventSource source, ExchangeItem exchangeItem = null)
            => new(GameResourceAction.Spent, currencyName, currencyAmount, source, exchangeItem);

        public static GameResourceParams Earned(CurrencyName currencyName, long currencyAmount, EventSource source, ExchangeItem exchangeItem = null)
            => new(GameResourceAction.Earned, currencyName, currencyAmount, source, exchangeItem);
        
        private GameResourceParams(GameResourceAction type, CurrencyName currencyName, long currencyAmount, EventSource source, ExchangeItem exchangeItem)
        {
            Type = type;
            CurrencyName = currencyName;
            CurrencyAmount = currencyAmount;
            Source = source;
            ExchangeItem = exchangeItem;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            _params ??= new Dictionary<string, object> (ExParams)
            {
                { "type", Type.Value },
                { "currency_name", CurrencyName.Value },
                { "currency_amount", CurrencyAmount },
                { "source", Source.Value }
            };
            if (ExchangeItem != null) _params["exchange_item"] = ExchangeItem.Value;
            return _params;
        }
    }
}