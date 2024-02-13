import { webcrypto } from "crypto";
import open from "open";

// Change these depending on the client you've created in APS.

const CLIENT_ID = Bun.env.CLIENT_ID || "CLIENT_ID";

const REDIRECT_URI = new URL("http://localhost:8080/oauth/callback/");
const EDITOR_SCOPES = ["data:read", "data:write"];
const VIEWER_SCOPES = ["data:read"];

function base64Url(buf: ArrayBufferLike) {
  return btoa(String.fromCharCode(...new Uint8Array(buf)))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/, "");
}

const code_verifier = base64Url(
  webcrypto.getRandomValues(new Uint8Array(32)).buffer,
);
const code_challenge = base64Url(
  await webcrypto.subtle.digest("SHA-256", Buffer.from(code_verifier)),
);

const authorizationEndpoint =
  "https://developer.api.autodesk.com/authentication/v2/authorize";

const query = new URLSearchParams({
  client_id: CLIENT_ID,
  response_type: "code",
  redirect_uri: REDIRECT_URI.toString(),
  scope: EDITOR_SCOPES.join(" "),
  nonce: "123123123",
  // prompt: "login", // Forces a new session, useful if you want to authorize the client with a different user
  code_challenge: code_challenge,
  code_challenge_method: "S256",
});

const authorizeUri = authorizationEndpoint + "?" + query.toString();

type TokenResponse = {
  access_token: string;
  refresh_token: string;
  expires_in: number;
  token_type: string;
};

const tokenResponsePromise = new Promise<TokenResponse>((res, rej) => {
  Bun.serve({
    port: REDIRECT_URI.port,
    fetch(req, server) {
      const url = new URL(req.url);
      if (url.pathname === REDIRECT_URI.pathname) {
        const code = url.searchParams.get("code");
        if (!code) {
          return new Response("/oauth/callback called with no code", {
            status: 500,
          });
        }
        console.log(code);
        const body = new URLSearchParams({
          grant_type: "authorization_code",
          client_id: CLIENT_ID,
          code_verifier: code_verifier,
          code: code,
          redirect_uri: REDIRECT_URI.toString(),
        });
        fetch("https://developer.api.autodesk.com/authentication/v2/token", {
          method: "POST",
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            accept: "application/json",
          },
          body: body.toString(),
        })
          .then((res) => {
            if (!res.ok) {
              console.error(
                `authorization code exchange failed: ${res.status} ${res.statusText}`,
              );
            }
            return res.json();
          })
          .then((body) => {
            server.stop();
            res(body);
          });
        return new Response("Success, you can close this tab now!", {
          status: 200,
        });
      }
      return new Response("Not Found", { status: 404 });
    },
  });
});

open(authorizeUri);

const tokenResponse = await tokenResponsePromise;

// Use the token however you want, console.log and copy to Bruno/Postman/whatever, or just make requests with fetch in this file.
console.log(tokenResponse);


// Save token response to a JSON file in the root directory
const saveTokenToFile = async (tokenData: TokenResponse) => {
  const filePath = 'token_response.json'; // Define the file path
  try {
    await Bun.write(filePath, JSON.stringify(tokenData, null, 2)); // Write the JSON data to the file
    console.log(`Token response saved to ${filePath}`);
  } catch (error) {
    console.error(`Failed to save token response to ${filePath}:`, error);
  }
};

// Call the function to save the token response
await saveTokenToFile(tokenResponse);
