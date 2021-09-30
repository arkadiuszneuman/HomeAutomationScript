#!/bin/bash
 echo "=====================Removing containers========================"
 docker rm -f $(docker ps -aq)