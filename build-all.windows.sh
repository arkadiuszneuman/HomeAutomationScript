#!/bin/bash
 echo "=====================Starting ELK stack========================"``
 [ -d elasticsearchdata ] || mkdir elasticsearchdata
 chmod g+rwx elasticsearchdata
 chgrp 1000 elasticsearchdata
 docker-compose -f ./docker-compose-elk.windows.yaml up -d
 echo "=====================Starting========================"
 docker-compose -f ./docker-compose.dependencies.yaml run --rm start_dependencies
 docker-compose -f ./docker-compose.windows.yaml up