#!/bin/bash
set -e

REGION="ap-southeast-1"

########################################
# STEP 1: Upload จาก PC → S3 (Key A)
########################################
export AWS_ACCESS_KEY_ID="AKIA_S3_XXXXXXXX"
export AWS_SECRET_ACCESS_KEY="S3_SECRET_XXXXXXXX"
unset AWS_SESSION_TOKEN
export AWS_DEFAULT_REGION="$REGION"

LOCAL_FILE="/c/temp/app.zip"   # C:\temp\app.zip (Git Bash path)
S3_BUCKET="my-bucket"
S3_KEY="deploy/app.zip"

echo "⬆️ Uploading file to S3..."
aws s3 cp "$LOCAL_FILE" "s3://$S3_BUCKET/$S3_KEY"

########################################
# STEP 2: สั่ง Windows EC2 (Key B)
########################################
export AWS_ACCESS_KEY_ID="AKIA_EC2_XXXXXXXX"
export AWS_SECRET_ACCESS_KEY="EC2_SECRET_XXXXXXXX"
unset AWS_SESSION_TOKEN
export AWS_DEFAULT_REGION="$REGION"

INSTANCE_ID="i-0123456789abcdef0"

echo "🚀 Sending SSM command to EC2..."
aws ssm send-command \
  --instance-ids "$INSTANCE_ID" \
  --document-name "AWS-RunPowerShellScript" \
  --comment "Copy from S3 to C:\\temp" \
  --parameters commands="
    if (!(Test-Path C:\temp)) { New-Item -ItemType Directory C:\temp }
    aws s3 cp s3://$S3_BUCKET/$S3_KEY C:\temp\app.zip
  "
