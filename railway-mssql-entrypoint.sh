#!/usr/bin/env bash
set -e

mkdir -p /var/opt/mssql/data \
    /var/opt/mssql/log \
    /var/opt/mssql/secrets \
    /var/opt/mssql/.system \
    /tmp/mssql-dumps \
    /.system

chmod -R 777 /var/opt/mssql /tmp/mssql-dumps /.system

exec /opt/mssql/bin/sqlservr
