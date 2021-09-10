#!/bin/sh

if [ $# -eq 0 ]; then

chown zidium /zidium/sqlite
chown zidium /zidium/log

_term() { 
  echo "Stopping all..." 
  kill -TERM "$child_dispatcher"
  kill -TERM "$child_user_account"
  kill -TERM "$child_agent"
}

trap _term SIGTERM

echo "Starting dispatcher...";
chroot --userspec=zidium / sh -c 'cd zidium/Dispatcher && dotnet Zidium.Dispatcher.dll' &
child_dispatcher=$!
echo "Dispatcher started, $child_dispatcher";

echo "Starting user account...";
chroot --userspec=zidium / sh -c 'cd zidium/UserAccount && dotnet Zidium.UserAccount.dll' &
child_user_account=$!
echo "User account started, $child_user_account";

echo "Starting agent...";
chroot --userspec=zidium / sh -c 'cd zidium/Agent && dotnet Zidium.Agent.dll --service' &
child_agent=$!
echo "Agent started, $child_agent";

echo "Ready to work!";
 
wait "$child_dispatcher"
echo "Dispatcher stopped"

wait "$child_user_account"
echo "User account stopped"

wait "$child_agent"
echo "Agent stopped"

else
chroot --userspec=zidium / "$@"
fi