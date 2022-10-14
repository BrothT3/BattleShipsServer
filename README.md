# BattleShipsServer
Functions as the host for BattleShips (other repo) game, handles turns and manages current users,
fetching and posting to a RESTFul service for chat function.

Uses UDP and attempted a GameState design pattern, which caused troubles using the project template this is based on.

Game order is 
RESTFulBattleShips -> BattleShipsServer -> BattleShips x2
