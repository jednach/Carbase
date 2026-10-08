
import { Container } from "@cloudflare/containers";

export class CarbaseContainer extends Container {
    defaultPort = 8080;
    sleepAfter = "10m";

    async onStart() {
        console.log("Carbase container started");
    }
}

export default {
    async fetch(request, env) {
        if (request.method === "POST") {
            const ip = request.headers.get("CF-Connecting-IP");

            if (!ip) {
                return new Response("Client IP unavailable", {
                    status: 400
                });
            }

            const { success } = await env.CARBASE_RATE_LIMITER.limit({
                key: ip
            });

            if (!success) {
                return new Response("Too many requests", {
                    status: 429,
                    headers: { "Retry-After": "60" }
                });
            }
        }

        const container = env.CARBASE_CONTAINER.getByName("carbase");

        await container.start({
            envVars: {
                ASPNETCORE_ENVIRONMENT: "Production",
                ConnectionStrings__DefaultConnection:
                    env.ConnectionStrings__DefaultConnection,
                R2__AccountId: env.R2__AccountId,
                R2__AccessKeyId: env.R2__AccessKeyId,
                R2__SecretAccessKey: env.R2__SecretAccessKey,
                R2__BucketName: env.R2__BucketName
            }
        });

        return container.fetch(request);
    }
};
