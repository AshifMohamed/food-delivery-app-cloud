terraform {
  backend "azurerm" {
    resource_group_name  = "FoodDelivery_Dev"
    storage_account_name = "fooddeliverydev"
    container_name       = "tfstate"
    key                  = "3x6m//WOZidnpTeVHDmAa1F15gAn3mh222+61yiBaHqsXLRW8ZeOHBdUVxCqRXYPfSD06rjMSUCn+ASth5QIwg=="
  }
}

provider "azurerm" {
  features {}
}


locals {
  resource_group="app-grp"
  location="eastus"
  other="nones"
}

data "azurerm_key_vault" "fooddelivery-dev-vault" {
  name                = "fooddelivery-dev-vault"
  resource_group_name = "FoodDelivery_Dev"
}

data "azurerm_key_vault_secret" "cosmos_db_con" {
  name      = "COSMOSDB"
  key_vault_id = data.azurerm_key_vault.fooddelivery-dev-vault.id
}

data "azurerm_key_vault_secret" "orderqueue_con" {
  name      = "PickMeOrderQueue"
  key_vault_id = data.azurerm_key_vault.fooddelivery-dev-vault.id
}

resource "azurerm_resource_group" "resource_group"{
  name="${var.project}-${var.environment}-resource-group"
  location=var.location
}

resource "azurerm_storage_account" "storage_account" {
  name                     = "${var.project}${var.environment}storage"
  resource_group_name      = azurerm_resource_group.resource_group.name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${var.project}-${var.environment}-app-service-plan"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name

  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_function_app" "function_app_order" {
  name                       = "functionapporder20230108"
  location                   = var.location
  resource_group_name        = azurerm_resource_group.resource_group.name
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.storage_account.name
  storage_account_access_key = azurerm_storage_account.storage_account.primary_access_key
  version = "~3"
  app_settings = {
    COSMOSDB              = "${data.azurerm_key_vault_secret.cosmos_db_con.value}"
    PickMeOrderQueue      = "${data.azurerm_key_vault_secret.orderqueue_con.value}"
  }
  site_config {
    dotnet_framework_version = "v4.0"
  }
}

resource "azurerm_function_app" "function_app_restaurant" {
  name                       = "functionapprestaurant20230108"
  location                   = var.location
  resource_group_name        = azurerm_resource_group.resource_group.name
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.storage_account.name
  storage_account_access_key = azurerm_storage_account.storage_account.primary_access_key
  version = "~3"
  app_settings = {
    COSMOSDB              = "${data.azurerm_key_vault_secret.cosmos_db_con.value}"
  }
  site_config {
    dotnet_framework_version = "v4.0"
  }
}
