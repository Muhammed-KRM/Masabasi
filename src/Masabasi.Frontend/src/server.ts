import {
  AngularNodeAppEngine,
  createNodeRequestHandler,
  isMainModule,
  writeResponseToNodeResponse,
} from '@angular/ssr/node';
import express from 'express';
import { join } from 'node:path';

const browserDistFolder = join(import.meta.dirname, '../browser');

process.env['NG_ALLOWED_HOSTS'] = '*';

const app = express();
const angularApp = new AngularNodeAppEngine({
  allowedHosts: ['*']
});

/**
 * Example Express Rest API endpoints can be defined here.
 * Uncomment and define endpoints as necessary.
 *
 * Example:
 * ```ts
 * app.get('/api/{*splat}', (req, res) => {
 *   // Handle API request
 * });
 * ```
 */

/**
 * Serve static files from /browser
 */
app.use(
  express.static(browserDistFolder, {
    maxAge: '1y',
    index: false,
    redirect: false,
  }),
);

/**
 * Handle all other requests by rendering the Angular application.
 */
app.use((req, res, next) => {
  angularApp
    .handle(req)
    .then((response) =>
      response ? writeResponseToNodeResponse(response, res) : next(),
    )
    .catch(next);
});

/**
 * Start the server if this module is the main entry point, or it is ran via PM2.
 * The server listens on the port defined by the `PORT` environment variable, or defaults to 4000.
 */
if (isMainModule(import.meta.url) || process.env['pm_id']) {
  const primaryPort = Number(process.env['PORT']) || 4000;
  app.listen(primaryPort, () => {
    console.log(`Node Express server listening on http://localhost:${primaryPort}`);
  });

  // Geriye dönük uyumluluk: 4200 portunu da dinleyerek hem 4000 hem 4200 erişimini garanti altına al
  if (primaryPort === 4000) {
    try {
      const server4200 = app.listen(4200, () => {
        console.log(`Uyumluluk portu devrede: http://localhost:4200`);
      });
      server4200.on('error', () => {
        // Port 4200 meşgulse sessizce birincil port 4000 ile devam et
      });
    } catch {
      // sessizce geç
    }
  }
}

/**
 * Request handler used by the Angular CLI (for dev-server and during build) or Firebase Cloud Functions.
 */
export const reqHandler = createNodeRequestHandler(app);
