namespace OnlineShop.Host.ApiModels;

// ReSharper disable once ClassNeverInstantiated.Global
internal record CreateItemRequest(string Title, string Description, int Price, int Amount);