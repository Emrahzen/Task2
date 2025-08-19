import Link from 'next/link'

export default async function HomePage({ params: { locale } }: { params: { locale: string } }) {
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">
        {locale === 'tr' ? 'Task2 Mağaza\'ya Hoş Geldiniz' : 'Welcome to Task2 Shop'}
      </h1>
      <p className="text-gray-600">
        {locale === 'tr' 
          ? 'Next.js 14, next-intl ve RTK kullanarak geliştirilmiş basit bir e-ticaret frontend\'i.' 
          : 'A simple e-commerce frontend using Next.js 14, next-intl, and RTK.'
        }
      </p>
      <div className="flex gap-3">
        <Link className="underline" href={`/${locale}/products`}>
          {locale === 'tr' ? 'Ürünlere göz at' : 'Browse products'}
        </Link>
        <Link className="underline" href={`/${locale}/auth/register`}>
          {locale === 'tr' ? 'Hesap oluştur' : 'Create account'}
        </Link>
      </div>
    </div>
  )
}