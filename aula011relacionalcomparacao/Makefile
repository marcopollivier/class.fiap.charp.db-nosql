DOCKER_COMPOSE = docker compose -f docker-compose.yml
POSTGRES_USER = pguser
POSTGRES_DB = pedidosdb
POSTGRES_PASSWORD = pgpass
SQLSERVER_SA = sa
SQLSERVER_PASSWORD = SqlServer!Pass123

up:
	$(DOCKER_COMPOSE) up -d

down:
	$(DOCKER_COMPOSE) down -v

ps:
	$(DOCKER_COMPOSE) ps

logs:
	$(DOCKER_COMPOSE) logs -f --tail=100

psql:
	PGPASSWORD=$(POSTGRES_PASSWORD) psql -h localhost -U $(POSTGRES_USER) -d $(POSTGRES_DB)

sqlcmd:
	/opt/mssql-tools/bin/sqlcmd -S localhost -U $(SQLSERVER_SA) -P $(SQLSERVER_PASSWORD)

seed-postgres:
	PGPASSWORD=$(POSTGRES_PASSWORD) psql -h localhost -U $(POSTGRES_USER) -d $(POSTGRES_DB) -f init-postgres/02-crud.sql

seed-sqlserver:
	/opt/mssql-tools/bin/sqlcmd -S localhost -U $(SQLSERVER_SA) -P $(SQLSERVER_PASSWORD) -d master -i init-sqlserver/02-crud.sql

clean:
	$(DOCKER_COMPOSE) down -v
	docker volume rm aula011relacionalcomparacao_pgdata || true
	docker volume rm aula011relacionalcomparacao_mssqldata || true

.PHONY: up down ps logs psql sqlcmd seed-postgres seed-sqlserver clean
