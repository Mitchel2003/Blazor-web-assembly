{
  "mcpServers": {
    "memory": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-memory"
      ],
      "env": {
        "MEMORY_FILE_PATH": "C:\\Users\\herre\\Documents\\Blazor-web-assembly"
      }
    },
    "notion": {
      "command": "npx",
      "args": [
        "-y",
        "@notionhq/notion-mcp-server"
      ],
      "env": {
        "OPENAPI_MCP_HEADERS": "{\"Authorization\": \"Bearer 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\", \"Notion-Version\": \"2022-06-28\"}"
      }
    },
    "azure-sql": {
      "command": "npx",
      "args": [
        "-y",
        "supergateway",
        "--sse",
        "https://mcp.pipedream.net/a7242e80-9876-4eb6-b6ab-1be9c08bcabb/azure_sql"
      ]
    },
    "Web Scraping (firecrawl)": {
      "command": "npx",
      "args": ["-y", "firecrawl-mcp"],
      "env": {
        "FIRECRAWL_API_KEY": "fc-6ce02c0b45bd479d8c3cc74eeac32dc9"
      }
    }
  }
}