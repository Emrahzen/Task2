"use client"
import React, { ReactNode } from 'react'
import Link from 'next/link'
import { NextIntlClientProvider } from 'next-intl'
import { usePathname } from 'next/navigation'
import LogoutButton from '@/components/LogoutButton'
import LanguageSwitcher from '@/components/LanguageSwitcher'
import AuthSessionWatcher from '@/components/AuthSessionWatcher'
import { useSelector } from 'react-redux'
import { RootState } from '@/store'

function Header({ locale }: { locale: string }) {
  const cartItems = useSelector((state: RootState) => state.cart.items)
  // Avoid SSR/CSR mismatch by computing after mount
  const [totalItems, setTotalItems] = React.useState<number>(0)
  React.useEffect(() => {
    setTotalItems(cartItems.reduce((total, item) => total + (item.quantity || 1), 0))
  }, [cartItems])
  return (
    <header className="border-b bg-white shadow-sm">
      <div className="container mx-auto flex items-center justify-between h-16 px-4">
        <Link href={`/${locale}/products`} className="text-xl font-bold text-gray-900">
          Task2 Shop
        </Link>
        <nav className="flex items-center gap-6 text-sm">
          <Link href={`/${locale}/products`} className="text-gray-700 hover:text-gray-900 transition-colors">
            {locale === 'tr' ? 'Ürünler' : 'Products'}
          </Link>
          <Link href={`/${locale}/cart`} className="text-gray-700 hover:text-gray-900 transition-colors relative">
            {locale === 'tr' ? 'Sepet' : 'Cart'}
            <span
              suppressHydrationWarning
              className={`absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center ${totalItems > 0 ? '' : 'invisible'}`}
            >
              {totalItems}
            </span>
          </Link>
          <LanguageSwitcher currentLocale={locale} />
          <LogoutButton locale={locale} />
        </nav>
      </div>
    </header>
  )
}

export default function LocaleLayout({ children, params: { locale } }: { children: ReactNode, params: { locale: string } }) {
  const pathname = usePathname()
  const isAuthPage = pathname?.includes('/auth/')
  
  return (
    <NextIntlClientProvider locale={locale}>
      <div>
        <AuthSessionWatcher locale={locale} />
        {!isAuthPage && <Header locale={locale} />}
        <main className={isAuthPage ? "" : "container mx-auto py-6 px-4"}>
          {children}
        </main>
      </div>
    </NextIntlClientProvider>
  )
}