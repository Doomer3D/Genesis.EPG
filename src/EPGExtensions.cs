using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace Genesis.EPG
{
    /// <summary>
    /// расширения EPG
    /// </summary>
    public static class EPGExtensions
    {
        /// <summary>
        /// попытаться получить идентификатор объекта
        /// </summary>
        /// <param name="item"> значение </param>
        /// <param name="removeIfExists"> указывает, что необходимо удалить идентификатор, если он имеется </param>
        /// <returns></returns>
        public static int? TryGetID(this JObject item, bool removeIfExists)
        {
            if (item == default) throw new ArgumentNullException(nameof(item));

            if (item.TryGetValue("id", out var token))
            {
                try
                {
                    if (token is JValue value && value.Value == default) return default;
                    return token.Value<int>();
                }
                catch
                {
                    return default;
                }
                finally
                {
                    if (removeIfExists)
                    {
                        // удаляем
                        item.Remove("id");
                    }
                }
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// получить строку из объекта
        /// </summary>
        /// <param name="item"> значение </param>
        /// <param name="key"> ключ </param>
        /// <param name="removeIfExists"> указывает, что необходимо удалить ключ, если он имеется </param>
        /// <returns></returns>
        public static string GetString(this JObject item, string key, bool removeIfExists)
        {
            if (item == default) throw new ArgumentNullException(nameof(item));

            if (item.TryGetValue(key, out var token))
            {
                try
                {
                    if (token is JValue value && value.Value == default) return default;
                    return token.Value<string>();
                }
                catch
                {
                    throw new KeyNotFoundException($"Ключ {key}  имеет неверный тип: {token.Type}");
                }
                finally
                {
                    if (removeIfExists)
                    {
                        // удаляем
                        item.Remove(key);
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException($"Ключ не найден: {key}");
            }
        }

        /// <summary>
        /// получить число из объекта
        /// </summary>
        /// <param name="item"> значение </param>
        /// <param name="key"> ключ </param>
        /// <param name="removeIfExists"> указывает, что необходимо удалить ключ, если он имеется </param>
        /// <returns></returns>
        public static object GetInt32OrNull(this JObject item, string key, bool removeIfExists)
        {
            if (item == default) throw new ArgumentNullException(nameof(item));

            if (item.TryGetValue(key, out var token))
            {
                try
                {
                    if (token is JValue value && value.Value == default) return default;
                    return token.Value<int>();
                }
                catch
                {
                    return default;
                }
                finally
                {
                    if (removeIfExists)
                    {
                        // удаляем
                        item.Remove(key);
                    }
                }
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// получить представление 'extra'
        /// </summary>
        /// <param name="item"> значение </param>
        /// <returns></returns>
        public static string GetExtra(this JObject item)
        {
            if (item == default) throw new ArgumentNullException(nameof(item));
            return item.ToString();
        }
    }
}
