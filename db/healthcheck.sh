#!/bin/bash

root_password=$(cat ./run/secrets/db_root_password)
mysql -uroot -p"${root_password}" --execute 'SHOW DATABASES;'

exit $?;