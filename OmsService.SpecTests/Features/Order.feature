Feature: Order
As a trader I want to place order to invest into Iran Bourse market.


@FreshOmsDb
Scenario: Placing a buy order
	Given Mehdi is a trader with PamCode '18990075201781'
	When he places a buying order for the Shasta instrument with isin 'IRZ0012346' by price 1500 and quantity 20
	Then he sees his order in his orders
