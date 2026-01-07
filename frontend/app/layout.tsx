import type { Metadata } from "next";
import "./globals.css";
import { AuthProvider } from "@/context/AuthContext";
import { FeatureProvider } from "@/context/FeatureContext";

export const metadata: Metadata = {
  title: "PPM - Petrol Pump Management",
  description: "Multi-tenant SaaS for petrol pump management",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className="antialiased">
        <AuthProvider>
          <FeatureProvider>
            {children}
          </FeatureProvider>
        </AuthProvider>
      </body>
    </html>
  );
}
