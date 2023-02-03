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

resource "azurerm_resource_group" "app_grp"{
  name=local.resource_group
  location=local.location
}

resource "azurerm_storage_account" "functionstore230108" {
  name                     = "functionstore230108"
  resource_group_name      = azurerm_resource_group.app_grp.name
  location                 = azurerm_resource_group.app_grp.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "function_app_plan" {
  name                = "function-app-plan"
  location            = azurerm_resource_group.app_grp.location
  resource_group_name = azurerm_resource_group.app_grp.name

  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_function_app" "functionapporder20230108" {
  name                       = "functionapporder20230108"
  location                   = azurerm_resource_group.app_grp.location
  resource_group_name        = azurerm_resource_group.app_grp.name
  app_service_plan_id        = azurerm_app_service_plan.function_app_plan.id
  storage_account_name       = azurerm_storage_account.functionstore230108.name
  storage_account_access_key = azurerm_storage_account.functionstore230108.primary_access_key
  version = "~3"
  app_settings = {
    COSMOSDB              = "${data.azurerm_key_vault_secret.cosmos_db_con.value}"
    PickMeOrderQueue      = "${data.azurerm_key_vault_secret.orderqueue_con.value}"
  }
  site_config {
    dotnet_framework_version = "v4.0"
  }
}

resource "azurerm_function_app" "functionapprestaurant20230108" {
  name                       = "functionapprestaurant20230108"
  location                   = azurerm_resource_group.app_grp.location
  resource_group_name        = azurerm_resource_group.app_grp.name
  app_service_plan_id        = azurerm_app_service_plan.function_app_plan.id
  storage_account_name       = azurerm_storage_account.functionstore230108.name
  storage_account_access_key = azurerm_storage_account.functionstore230108.primary_access_key
  version = "~3"
  app_settings = {
    COSMOSDB              = "${data.azurerm_key_vault_secret.cosmos_db_con.value}"
  }
  site_config {
    dotnet_framework_version = "v4.0"
  }
}
