using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NoChainSwap.DTO.Transaction;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace NoChainSwap.Domain.Impl.Core
{
    public static class Utils
    {
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static void Copy<TParent, TChild>(TParent parent, TChild child)
        {
            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        try
                        {
                            childProperty.SetValue(child, parentProperty.GetValue(parent));
                        }
                        catch (Exception) { break; }
                        break;
                    }
                }
            }
        }

        public static CoinEnum StrToCoin(string coinStr)
        {
            CoinEnum ret = CoinEnum.Bitcoin;
            switch (coinStr)
            {
                case "btc":
                    ret = CoinEnum.Bitcoin;
                    break;
                case "stx":
                    ret = CoinEnum.Stacks;
                    break;
                case "usdt":
                    ret = CoinEnum.USDT;
                    break;
                case "brl":
                    ret = CoinEnum.BRL;
                    break;
                default:
                    throw new Exception($"{coinStr} is not a valid coin");
            }
            return ret;
        }

        public static string CoinToStr(CoinEnum coin)
        {
            string str = string.Empty;
            switch (coin)
            {
                case CoinEnum.Bitcoin:
                    str = "btc";
                    break;
                case CoinEnum.Stacks:
                    str = "stx";
                    break;
                case CoinEnum.USDT:
                    str = "usdt";
                    break;
                case CoinEnum.BRL:
                    str = "brl";
                    break;
            }
            return str;
        }

        public static string CoinToText(CoinEnum coin)
        {
            string str = string.Empty;
            switch (coin)
            {
                case CoinEnum.Bitcoin:
                    str = "Bitcoin (BTC)";
                    break;
                case CoinEnum.Stacks:
                    str = "Stacks (STX)";
                    break;
                case CoinEnum.USDT:
                    str = "Tether (USDT)";
                    break;
                case CoinEnum.BRL:
                    str = "Real (Pix)";
                    break;
            }
            return str;
        }

        public static string CoinToSlug(CoinEnum coin)
        {
            string str = string.Empty;
            switch (coin)
            {
                case CoinEnum.Bitcoin:
                    str = "bitcoin";
                    break;
                case CoinEnum.Stacks:
                    str = "stacks";
                    break;
                case CoinEnum.USDT:
                    str = "tether";
                    break;
                case CoinEnum.BRL:
                    str = "brl";
                    break;
            }
            return str;
        }

        public static string CurrencyToStr(CurrencyEnum currency)
        {
            string str = string.Empty;
            switch (currency)
            {
                case CurrencyEnum.USD:
                    str = "USD";
                    break;
                case CurrencyEnum.BRL:
                    str = "BRL";
                    break;
            }
            return str;
        }

        public static CurrencyEnum StrToCurrency(string currencyStr)
        {
            var ret = CurrencyEnum.USD;
            switch (currencyStr)
            {
                case "USD":
                    ret = CurrencyEnum.USD;
                    break;
                case "BRL":
                    ret = CurrencyEnum.BRL;
                    break;
                default:
                    throw new Exception($"{currencyStr} is not a valid currency");
            }
            return ret;
        }
    }
}
