import './globals.css'
import { ReactNode } from 'react'
import { Providers } from '@/store/providers'

export const metadata = {
  title: 'Task2 Shop',
  description: 'SEO-friendly e-commerce frontend'
}

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body>
        <Providers>
          {children}
        </Providers>
      </body>
    </html>
  )
}