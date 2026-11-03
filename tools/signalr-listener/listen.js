// FamilyCart SignalR listener (test tool, not app code)
//
// Logs in as a user, connects to /hubs/family, joins a family group,
// and prints every list item event the server broadcasts.
//
// Usage:
//   node listen.js --email bob@test.com --password Passw0rd! --family 12
//   node listen.js --email bob@test.com --password Passw0rd! --family 12 --host http://localhost:5208
//
// Stop with Ctrl+C.

const signalR = require("@microsoft/signalr");

// ---------------------------------------------------------------------------
// Arguments
// ---------------------------------------------------------------------------
function readArgs() {
  const args = {};
  const argv = process.argv.slice(2);
  for (let i = 0; i < argv.length; i += 2) {
    const key = argv[i];
    const value = argv[i + 1];
    if (!key.startsWith("--") || value === undefined) {
      usageAndExit(`Bad argument near "${key}".`);
    }
    args[key.slice(2)] = value;
  }

  if (!args.email || !args.password || !args.family) {
    usageAndExit("Missing --email, --password or --family.");
  }

  const familyId = Number(args.family);
  if (!Number.isInteger(familyId)) {
    usageAndExit(`--family must be a whole number, got "${args.family}".`);
  }

  return {
    email: args.email,
    password: args.password,
    familyId,
    host: (args.host || "http://localhost:5208").replace(/\/$/, ""),
  };
}

function usageAndExit(message) {
  console.error(message);
  console.error(
    "Usage: node listen.js --email <email> --password <password> --family <familyId> [--host http://localhost:5208]"
  );
  process.exit(1);
}

// ---------------------------------------------------------------------------
// Output helpers
// ---------------------------------------------------------------------------
function time() {
  return new Date().toLocaleTimeString();
}

function log(label, message) {
  console.log(`[${time()}] ${label.padEnd(9)} ${message}`);
}

function describeItem(item) {
  const check = item.isChecked ? "[x]" : "[ ]";
  const archived = item.isArchived ? "  (product archived)" : "";
  return (
    `${check} ${item.productName}  qty ${item.quantity}  $${item.price}` +
    `  | item ${item.id}, list ${item.shoppingListId}${archived}`
  );
}

// ---------------------------------------------------------------------------
// Login
// ---------------------------------------------------------------------------
async function login(host, email, password) {
  let response;
  try {
    response = await fetch(`${host}/api/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });
  } catch (err) {
    throw new Error(`Could not reach ${host}. Is the API running? (${err.message})`);
  }

  if (!response.ok) {
    const text = await response.text();
    throw new Error(`Login failed: ${response.status} ${text}`);
  }

  const body = await response.json();
  if (!body.token) {
    throw new Error("Login succeeded but the response had no token field.");
  }
  return body.token;
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
async function main() {
  const { email, password, familyId, host } = readArgs();

  log("LOGIN", `Logging in as ${email} ...`);
  const token = await login(host, email, password);
  log("LOGIN", "OK, got a token.");

  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${host}/hubs/family`, {
      // The client sends this as ?access_token=... on the WebSocket request,
      // which is what your OnMessageReceived handler reads.
      accessTokenFactory: () => token,
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  // Event names and argument order must match IFamilyClient.
  connection.on("ListItemAdded", (item) => log("ADDED", describeItem(item)));
  connection.on("ListItemUpdated", (item) => log("UPDATED", describeItem(item)));
  connection.on("ListItemDeleted", (itemId, shoppingListId) =>
    log("DELETED", `item ${itemId} from list ${shoppingListId}`)
  );

  // Groups are not remembered across reconnects, so join again.
  connection.onreconnecting(() => log("CONN", "Connection lost, reconnecting ..."));
  connection.onreconnected(async () => {
    log("CONN", "Reconnected, rejoining family ...");
    await joinFamily(connection, familyId);
  });
  connection.onclose((err) => {
    log("CONN", `Connection closed${err ? `: ${err.message}` : "."}`);
    process.exit(err ? 1 : 0);
  });

  log("CONN", `Connecting to ${host}/hubs/family ...`);
  try {
    await connection.start();
  } catch (err) {
    throw new Error(
      `Could not connect to the hub: ${err.message}\n` +
        "  401 = token rejected (check OnMessageReceived)\n" +
        "  404 = hub not mapped (check MapHub path)"
    );
  }
  log("CONN", `Connected (transport: ${connection.connection.transport?.constructor?.name ?? "unknown"}).`);

  await joinFamily(connection, familyId);
  log("READY", "Listening for events. Send requests from smoke-tests.http. Ctrl+C to stop.");

  process.on("SIGINT", async () => {
    log("CONN", "Stopping ...");
    await connection.stop();
  });
}

async function joinFamily(connection, familyId) {
  try {
    await connection.invoke("JoinFamily", familyId);
    log("JOIN", `Joined group for family ${familyId}.`);
  } catch (err) {
    // A HubException message from the server comes through here.
    log("JOIN", `FAILED: ${err.message}`);
    await connection.stop();
    process.exit(1);
  }
}

main().catch((err) => {
  console.error(`[${time()}] ERROR     ${err.message}`);
  process.exit(1);
});
