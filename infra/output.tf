output "bucket_name" {
  value = aws_s3_bucket.videos.bucket
}

output "table_name" {
  value = aws_dynamodb_table.video_metadata.name
}