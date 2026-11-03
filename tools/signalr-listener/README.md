# SignalR listener (test tool)

Connects to the FamilyCart hub as a user, joins a family, and prints every
list item event the server broadcasts. It uses `@microsoft/signalr`, the same
client library the Expo app will use.

## Setup (once)

From this folder:

    npm install

## Run

    node listen.js --email bob@test.com --password Passw0rd! --family <familyId>

Optional: `--host http://localhost:5208` (that's the default).

Stop with Ctrl+C.

## Test walkthrough

Start the API first, then work through `smoke-tests.http`.

1. **Get the family ID.** Run the smoke tests through **2a** (Alice creates a family).
   The response has the `id` you need.

2. **Not a member yet.** Start the listener as **Bob** with Alice's family ID.
   Expect:

       JOIN      FAILED: ... HubException: Not a member of this family

3. **Invite Bob.** In `smoke-tests.http`, run **8a** (Alice adds Bob by email).
   You can run it out of order, as long as 2a has run.

4. **Listen.** Start the listener as Bob again. Expect `Joined group for family ...`
   and `Listening for events`.

5. **Make changes as Alice** and watch Bob's terminal:

   | Request | Expected event |
   |---|---|
   | 6a / 6d add an item | `ADDED   [ ] Ground Beef  qty 2 ...` |
   | 6b / 7b / 9h check or uncheck | `UPDATED [x] ...` or `UPDATED [ ] ...` |
   | 9h after 9a archived beef | `UPDATED ... (product archived)` |
   | 10a / 10b delete an item | `DELETED item 5 from list 3` |

   Failed requests (403, 404) should produce **no** event.

## If something goes wrong

- `Could not reach ...`: the API isn't running, or the host or port is wrong.
- `Login failed: 401`: wrong email or password, or the user isn't registered (run 1a/1b).
- Hub connect fails with 401: the token isn't being read from the query string (check `OnMessageReceived`).
- Hub connect fails with 404: the hub path doesn't match `MapHub` (`/hubs/family`).
- Connected and joined but no events: check that the requests are for a list in
  the same family ID you passed, and that the controller broadcasts after `SaveChangesAsync()`.
