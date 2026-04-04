export async function postJson<TReq, TRes>(url: string, body: TReq): Promise<TRes> {
  const res: Response = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  })

  if (!res.ok) {
    const err: unknown = await res.json().catch(() => ({}))
    throw err
  }

  const parsed = (await res.json()) as TRes
  return parsed
}
