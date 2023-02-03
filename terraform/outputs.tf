# terraform/outputs.tf

output "function_app_order_name" {
  value = azurerm_function_app.function_app_order.name
  description = "Deployed function app name"
}

output "function_app_order_default_hostname" {
  value = azurerm_function_app.function_app_order.default_hostname
  description = "Deployed function app hostname"
}

output "function_app_rest_name" {
  value = azurerm_function_app.function_app_restaurant.name
  description = "Deployed function app name"
}

output "function_app_rest_default_hostname" {
  value = azurerm_function_app.function_app_restaurant.default_hostname
  description = "Deployed function app hostname"
}
