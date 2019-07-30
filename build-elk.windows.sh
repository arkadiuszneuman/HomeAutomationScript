#!/bin/bash
 echo "=====================Starting ELK stack========================"``
 [ -d elasticsearchdata ] || mkdir elasticsearchdata
 chmod g+rwx elasticsearchdata
 chgrp 1000 elasticsearchdata
 docker-compose -f ./docker-compose-elk.windows.yaml up