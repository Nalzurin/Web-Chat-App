export default function Footer() {
  return (
    <footer className="border-t p-4 mt-10">
      <div className="container mx-auto text-center text-sm text-muted-foreground">
        © {new Date().getFullYear()} ChatApp. All rights reserved.
      </div>
    </footer>
  )
}
